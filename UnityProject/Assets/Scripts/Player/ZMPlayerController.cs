using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ZMPlayer;
using Core;

[RequireComponent(typeof(ZMPlayerInputController))]
public class ZMPlayerController : ZMPlayerItem
{
	// Movement constants.
	private float GRAVITY = 2200.0f;
	private float RUN_SPEED_MAX = 500.0f;
	private float ACCELERATION = 30.0f;
	private float FRICTION = 25.0f;
	private float JUMP_HEIGHT = 800.0f;
	private float PLUNGE_SPEED = 2000.0f;
	private float LUNGE_SPEED = 1600.0f;
	private float LUNGE_TIME = 0.12f;
	private float WALL_SLIDE_SPEED = 80.0f;
	private float WALL_JUMP_KICK_SPEED = 500.0f;
	private float EDGE_OFFSET = 16.0f;
	private float AOE_RANGE = 32.0f;
	private float RECOIL_STUN_TIME = 0.001f;
	private float PARRY_TIME_LUNGE = 0.30f;
	private float PARRY_TIME = 0.5f;
	private float PARRY_STUN_WINDOW = 0.25f;
	private float PARRY_TIME_AIR = 0.10f;
	private float STUN_TIME = 0.6f;
	private float runSpeed = 0.0f;

	// Additional constants.
	public float RESPAWN_TIME = 5.0f;
	private float TILE_SIZE = 2.0f;
	private int FRAMES_PER_STEP = 30;

	private CharacterController2D _controller;
	private ZMPlayerInputController _inputController;

	private RaycastHit2D _lastControllerColliderHit;
	private RaycastHit2D _checkTouchingLeft;
	private RaycastHit2D _checkTouchingRight;

	private Vector3 _velocity;
	private Vector3 _storedVelocity;

	private int _framesUntilStep = 0;
	private string[] kDeathStrings;
	private bool _canLunge;
	private bool _canWallJump;
	private bool _canAirLunge;

	// Speeds of two players before recoil.
	private Vector2 _collisionVelocities;

	// Player states.
	private enum MovementDirectionState { FACING_LEFT, FACING_RIGHT };
	private enum ControlMoveState 		{ NEUTRAL, MOVING };
	private enum ControlModState	    { NEUTRAL, JUMPING, ATTACK, AIR_ATTACK, ATTACKING, WALL_JUMPING, PLUNGE, PLUNGING, PARRY, PARRYING };
	private enum MoveModState 		    { NEUTRAL, PLUNGE, PLUNGING, LUNGE, LUNGING_AIR, LUNGING_GROUND, WALL_SLIDE, RECOIL, RECOILING, STUN, STUNNED, DISABLE, DISABLED, PARRY_FACING, PARRY_AOE, RESPAWN, ELIMINATED };
	private enum AbilityState 			{ NEUTRAL, SHOOTING };

	private ControlMoveState _controlMoveState;
	private ControlModState _controlModState;
	private MovementDirectionState _movementDirection;
	private MoveModState _moveModState;
	private bool _playerInPath;
	private bool _canStun;

	// Tags and layers.
	private const string kPlayerTag						    = "Player";
	private const string kWarpVolumeTag						= "WarpVolume";
	private const string kGroundLayerMaskName 			    = "Ground";
	private const string kSpecialInteractiblesLayerMaskName = "SpecialInteractibles";
	
	// Methods.
	private const string kMethodNameEnablePlayer  			= "EnablePlayer";
	private const string kMethodNameEndLunge 	  			= "EndLunge";
	private const string kEndParryMethodName				= "EndParry";
	private const string kGameStateControllerName 			= "GameController";

	// Protected references.
	protected Animator _animator;

	// Public references.
	public GameObject _effectJumpObject;
	public GameObject _effectLandObject;
	public GameObject _effectLungeObject;
	public GameObject _effectSkidObject;
	public GameObject _effectClashObject;
	public GameObject _effectPlungeObject;
	public ParticleSystem _goreEmitter;
	public Text _tauntText;

	// Sound resources.
	public AudioClip[] _audioJump;
	public AudioClip[] _audioHurt;
	public AudioClip[] _audioGore;
	public AudioClip[] _audioStep;
	public AudioClip[] _audioLand;
	public AudioClip[] _audioKill;
	public AudioClip[] _audioSword;
	public AudioClip[] _audioLunge;
	public AudioClip[] _audioPlunge;
	public AudioClip[] _audioBash;
	public AudioClip _audioRecoil;
	public AudioClip _audioParry;
	public AudioClip _audioLungeFail;

	// DISMEMBERMENT!
	private const string kBodyUpperHalfPath = "PlayerHalfUpper";
	private const string kBodyLowerHalfPath = "PlayerHalfLower";
	private GameObject _lowerBodyTemplate, _upperBodyTemplate;

	// Materials
	private Material _materialDefault;
	public Material _materialFlash;

	// Delegates
	public static EventHandler<ZMPlayerController> OnPlayerCreate;
	public static EventHandler<ZMPlayerController> PlayerKillEvent;
	public static EventHandler<ZMPlayerInfo> PlayerDeathEvent;
	public static EventHandler<ZMPlayerController> PlayerRespawnEvent;
	public static EventHandler<ZMPlayerController> PlayerEliminatedEvent;
	public static EventHandler<ZMPlayerController, float> PlayerRecoilEvent;
	public static EventHandler<ZMPlayerController, float> PlayerStunEvent;
	public static EventHandler<ZMPlayerController, float> PlayerParryEvent;
	public static EventHandler PlayerLandPlungeEvent;

	// Debug
	private SpriteRenderer _spriteRenderer;
	private Color _baseColor;

	protected override void Awake()
	{
		base.Awake();

		_moveModState = MoveModState.NEUTRAL;
		transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		GetComponentReferences();

		_canLunge = true;
		_canWallJump = true;
		_canAirLunge = true;

		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		ZMStageScoreController.OnReachMinScore += HandleMinScoreReached;

		MatchStateManager.OnMatchPause += HandleOnMatchPause;
		MatchStateManager.OnMatchResume += HandleOnMatchResume;
		MatchStateManager.OnMatchStart += HandleOnMatchStart;
		MatchStateManager.OnMatchEnd += HandleOnMatchPause;

		// load resources
		_upperBodyTemplate = Resources.Load(kBodyUpperHalfPath, typeof(GameObject)) as GameObject;
		_lowerBodyTemplate = Resources.Load(kBodyLowerHalfPath, typeof(GameObject)) as GameObject;
	}

	protected virtual void Start()
	{
		kDeathStrings = new string[39];
		kDeathStrings[0] = "OOOAHH";
		kDeathStrings[1] = "WHOOOP";
		kDeathStrings[2] = "AYYYEEH";
		kDeathStrings[3] = "HADOOOP";
		kDeathStrings[4] = "WHUAAAH";
		kDeathStrings[5] = "BLARHGH";
		kDeathStrings[6] = "OUCH";
		kDeathStrings[7] = "DUN GOOFD";
		kDeathStrings[8] = "REKT";
		kDeathStrings[9] = "PWNED";
		kDeathStrings[10] = "SPLAT";
		kDeathStrings[11] = "SPLUUSH";
		kDeathStrings[12] = "ASDF";
		kDeathStrings[13] = "WHAAUH";
		kDeathStrings[14] = "AUUGH";
		kDeathStrings[15] = "WAOOOH";
		kDeathStrings[16] = "DERP";
		kDeathStrings[17] = "DISGRACE";
		kDeathStrings[18] = "DISHONOR";
		kDeathStrings[19] = "HUUUAP";
		kDeathStrings[20] = "PUUUAH";
		kDeathStrings[21] = "AYUUSH";
		kDeathStrings[22] = "WYAAAH";
		kDeathStrings[23] = "KWAAAH";
		kDeathStrings[24] = "HUZZAH";
		kDeathStrings[25] = "#WINNING";
		kDeathStrings[26] = "NOOB";
		kDeathStrings[27] = "ELEGANT";
		kDeathStrings[28] = "SWIFT";
		kDeathStrings[29] = "WAHH";
		kDeathStrings[30] = "OOOOOOHH";
		kDeathStrings[31] = "POOOOW";
		kDeathStrings[32] = "YAAAAS";
		kDeathStrings[33] = "SWOOOP";
		kDeathStrings[34] = "LOLWUT";
		kDeathStrings[35] = "SMOOTH";
		kDeathStrings[36] = "YUUUUS";
		kDeathStrings[37] = "YEESSS";
		kDeathStrings[38] = "NOICE";

		// Set original facing direction.
		SetMovementDirection(transform.position.x > 0 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);

		_baseColor = GetComponent<Light>().color;
		_goreEmitter.GetComponent<Renderer>().material.color = _baseColor;
		_goreEmitter.startColor = _baseColor;

		_materialDefault = GetComponent<Renderer>().material;

		Notifier.SendEventNotification(OnPlayerCreate, this);
	}

	void Update()
	{
		// Set raycasts.
		_checkTouchingLeft  = CheckLeft(TILE_SIZE, _controller.platformMask);
		_checkTouchingRight = CheckRight(TILE_SIZE, _controller.platformMask);

		// Update the velocity calculated from the controller.
		_velocity = _controller.velocity;
		
		// Check raycasts.
		if (_controller.isGrounded) {
			// Landing.
			if (_velocity.y < -1) {
				GetComponent<AudioSource>().PlayOneShot(_audioLand[Random.Range (0, _audioLand.Length)], 0.5f);
				Instantiate(_effectLandObject, new Vector2(transform.position.x - 3, transform.position.y - 8), transform.rotation);
			}

			_velocity.y = 0;

			if (IsPerformingPlunge()) {
				GetComponent<AudioSource>().PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);

				Notifier.SendEventNotification(PlayerLandPlungeEvent);
			}
		}

		// Horizontal movement.
		if (_controlMoveState == ControlMoveState.MOVING && !IsPerformingPlunge() && !IsRecoiling()) {

			if (_movementDirection == MovementDirectionState.FACING_RIGHT) {
				if (runSpeed < RUN_SPEED_MAX) {
					runSpeed += ACCELERATION;
				}
			} else if (_movementDirection == MovementDirectionState.FACING_LEFT) {
				if (runSpeed > -RUN_SPEED_MAX) {
					runSpeed -= ACCELERATION;
				}
			}

			if (_controller.isGrounded && Mathf.Abs(_velocity.x) > ACCELERATION) {
				_framesUntilStep++;
				if (_framesUntilStep >= FRAMES_PER_STEP) {
					_framesUntilStep = 0;
					GetComponent<AudioSource>().PlayOneShot(_audioStep[Random.Range (0, _audioStep.Length)], 0.25f);
				}
			}
		}

		// Friction.
		if (Mathf.Abs(runSpeed) <= FRICTION) {
			runSpeed = 0.0f;
		} else {
			if (_controlMoveState == ControlMoveState.NEUTRAL) {
				runSpeed -= FRICTION * Mathf.Sign(runSpeed);
			}
		}

		// Jumping.
		if (_controlModState == ControlModState.JUMPING) {
			_controlModState = ControlModState.NEUTRAL;

			_velocity.y = JUMP_HEIGHT;

			GetComponent<AudioSource>().PlayOneShot(_audioJump[Random.Range (0, _audioJump.Length)]);
			Instantiate(_effectJumpObject, new Vector2(transform.position.x - 3, transform.position.y - 8), transform.rotation);
		}

		// Update movement and ability state.
		if (_controlModState == ControlModState.ATTACK)
		{
			if (!IsPerformingLunge() && _canLunge)
			{
				HorizontalAttack();
			}
			else
			{
				GetComponent<AudioSource>().PlayOneShot(_audioLungeFail);
			}
		}
		else if (_controlModState == ControlModState.AIR_ATTACK)
		{
			if (!IsPerformingLunge() && _canLunge && _canAirLunge)
			{
				_canAirLunge = false;
				HorizontalAttack();
			}
			else
			{
				GetComponent<AudioSource>().PlayOneShot(_audioLungeFail);
			}
		}
		else if (_controlModState == ControlModState.PLUNGE)
		{
			_controlModState = ControlModState.PLUNGING;

			if (!IsPerformingPlunge())
			{
				GetComponent<AudioSource>().PlayOneShot(_audioPlunge[Random.Range (0, _audioPlunge.Length)]);
				_moveModState = MoveModState.PLUNGE;
			}
		}
		else if (_controlModState == ControlModState.PARRY)
		{
			_controlModState = ControlModState.PARRYING;
			_moveModState = MoveModState.PARRY_FACING;
			_canStun = true;
			_canLunge = false;

			GameObject effect = Instantiate(_effectClashObject,
			                                new Vector2(transform.position.x, transform.position.y),
			                                transform.rotation) as GameObject;
			effect.transform.parent = transform;
			effect.transform.localScale = new Vector3(0.66f, 0.66f, 1.0f);

			GetComponent<Renderer>().material = _materialFlash;
			GetComponent<AudioSource>().PlayOneShot(_audioParry);

			if (_controller.isGrounded)
			{
				Invoke("EndStunBeginParry", PARRY_STUN_WINDOW);
				DisableInputWithCallbackDelay(PARRY_STUN_WINDOW + PARRY_TIME);

				Notifier.SendEventNotification(PlayerParryEvent, this, PARRY_STUN_WINDOW + PARRY_TIME);
			}
		}
		else if (_controlModState == ControlModState.NEUTRAL)
		{
			if (_controller.isGrounded) { _canAirLunge = true; }

			if (IsTouchingEitherSide())
			{
				if (!_controller.isGrounded && _moveModState == MoveModState.NEUTRAL) { _moveModState = MoveModState.WALL_SLIDE; }
			}
		}

		if (_moveModState == MoveModState.PLUNGE)
		{
			_moveModState = MoveModState.PLUNGING;
			Plunge();
		}
		else if (_moveModState == MoveModState.PLUNGING)
		{
			if (_controller.isGrounded)
			{
				Instantiate(_effectPlungeObject, new Vector2(transform.position.x, transform.position.y), transform.rotation);

				// AOE Check:
				ZMPlayerController playerController;

				// Check right:
				RaycastHit2D recoilAOE = CheckRight(new Vector2(EDGE_OFFSET, -16.0f), AOE_RANGE, _controller.specialInteractibleMask);
				if (recoilAOE)
				{
					playerController = recoilAOE.collider.GetComponent<ZMPlayerController>();

					if (playerController != null)
					{
						playerController._movementDirection = MovementDirectionState.FACING_LEFT;
						playerController._moveModState = MoveModState.RECOIL;
					}
				}
				// Check left:
				recoilAOE = CheckLeft(new Vector2(-EDGE_OFFSET, -16.0f), AOE_RANGE, _controller.specialInteractibleMask);

				if (recoilAOE)
				{
					playerController = recoilAOE.collider.GetComponent<ZMPlayerController>();

					if (playerController != null)
					{
						playerController._movementDirection = MovementDirectionState.FACING_RIGHT;
						playerController._moveModState = MoveModState.RECOIL;
					}
				}

				_moveModState = MoveModState.PARRY_AOE;
				DisablePlayer();

				Invoke(kMethodNameEnablePlayer, PARRY_TIME_LUNGE);
				Invoke("ResetControlModState", PARRY_TIME_LUNGE + 0.02f);
			}
		}
		else if (_moveModState == MoveModState.LUNGE)
		{
			_moveModState = _controller.isGrounded ? MoveModState.LUNGING_GROUND : MoveModState.LUNGING_AIR;

			RaycastHit2D checkPlayer;

			if (_movementDirection == MovementDirectionState.FACING_RIGHT)
			{
				if (checkPlayer = CheckRight(145f, _controller.specialInteractibleMask))
				{
					if (checkPlayer.collider.CompareTag(kPlayerTag) && !_playerInPath)
					{
						_playerInPath = true;
					}
				}

				LungeRight();
			}
			else if (_movementDirection == MovementDirectionState.FACING_LEFT)
			{
				if (checkPlayer = CheckLeft(145f, _controller.specialInteractibleMask))
				{
					if (checkPlayer.collider.CompareTag(kPlayerTag) && !_playerInPath)
					{
						_playerInPath = true;
					}
				}

				LungeLeft();
			}

			// End the lunge after a delay
			if (IsInvoking(kMethodNameEndLunge)) { CancelInvoke(kMethodNameEndLunge); }
			Invoke (kMethodNameEndLunge, LUNGE_TIME);
		}

		if (_movementDirection == MovementDirectionState.FACING_RIGHT && (_moveModState == MoveModState.LUNGING_AIR || _moveModState == MoveModState.LUNGING_GROUND)) {
			RaycastHit2D hit = CheckRight (4, _controller.platformMask);
			if (hit && !hit.collider.CompareTag("Breakable")) {
				GetComponent<AudioSource>().PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
				Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 90.0f));
				Instantiate(_effectPlungeObject, new Vector2(transform.position.x - 18, transform.position.y), rotation);
				EndLunge ();
			}
		}
		if (_movementDirection == MovementDirectionState.FACING_LEFT && (_moveModState == MoveModState.LUNGING_AIR || _moveModState == MoveModState.LUNGING_GROUND)) {
			RaycastHit2D hit = CheckLeft (4, _controller.platformMask);
			if (hit && !hit.collider.CompareTag("Breakable")) {
				GetComponent<AudioSource>().PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
				Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 270.0f));
				Instantiate(_effectPlungeObject, new Vector2(transform.position.x + 14, transform.position.y), rotation);
				EndLunge ();
			}
		}

		
		if (_moveModState == MoveModState.RECOIL) {
			GetComponent<AudioSource>().PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
			Instantiate(_effectClashObject, new Vector2(transform.position.x, transform.position.y), transform.rotation);
			_moveModState = MoveModState.RECOILING;

			if (IsInvoking(kMethodNameEndLunge)) CancelInvoke(kMethodNameEndLunge);

			Recoil();

			DisableInputWithCallbackDelay(RECOIL_STUN_TIME);

			Notifier.SendEventNotification(PlayerRecoilEvent, this, RECOIL_STUN_TIME);
		} else if (_moveModState == MoveModState.RECOILING) {
			_moveModState = MoveModState.NEUTRAL;
			_controlModState = ControlModState.NEUTRAL;
			_controlMoveState = ControlMoveState.NEUTRAL;

			_playerInPath = false;
		} else if (_moveModState == MoveModState.STUN) {
			_moveModState = MoveModState.STUNNED;
			_controlModState = ControlModState.NEUTRAL;
			_controlMoveState = ControlMoveState.NEUTRAL;
			GetComponent<AudioSource>().PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);

			if (IsInvoking(kMethodNameEndLunge)) CancelInvoke(kMethodNameEndLunge);

			Recoil();
			Invoke("ResetMoveModState", STUN_TIME);
			DisableInputWithCallbackDelay(STUN_TIME);


			Notifier.SendEventNotification(PlayerStunEvent, this, STUN_TIME);
		} else if (_moveModState == MoveModState.WALL_SLIDE) {
			// Wall slide.
			if (_velocity.y < 1.0f &&  _controlMoveState == ControlMoveState.MOVING) {
				_velocity.y = -WALL_SLIDE_SPEED;
				if (IsTouchingRightAndMovingRight() || IsTouchingLeftAndMovingLeft()) {
					runSpeed = 0;
				}
			}
			if (_controller.isGrounded || !IsTouchingEitherSide()) {
				_moveModState = MoveModState.NEUTRAL;
			}
			// Wall jump.
			if (_controlModState == ControlModState.WALL_JUMPING) {
				_controlModState = ControlModState.NEUTRAL;

				if (IsMovingLeft () || IsMovingRight()) {
					if (!_controller.isGrounded) {
						_velocity.y = JUMP_HEIGHT;
						GetComponent<AudioSource>().PlayOneShot(_audioJump[Random.Range (0, _audioJump.Length)]);
						Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 180.0f : 0.0f), 0.0f));
						float offset = (_movementDirection == MovementDirectionState.FACING_RIGHT ? 12.0f : -12.0f);
						Instantiate (_effectSkidObject, new Vector2 (transform.position.x + offset, transform.position.y - 20), rotation);
						_moveModState = MoveModState.NEUTRAL;
						_canWallJump = false;
						_canAirLunge = true;
						Invoke("WallJumpCooldown", 0.05f);

						if (IsMovingLeft()) {
							runSpeed = WALL_JUMP_KICK_SPEED * 0.6f;
						}
						else if (IsMovingRight()) {
							runSpeed = -WALL_JUMP_KICK_SPEED * 0.6f;
						}
					}
				}
			}
		}

		// Update visuals.
		if (!_controller.isGrounded && !_canAirLunge) {
			Color color = GetComponent<Renderer>().material.color;
			color.a = 0.5f;
			GetComponent<Renderer>().material.color = color;
		} else {
			Color color = GetComponent<Renderer>().material.color;
			color.a = 1.0f;
			GetComponent<Renderer>().material.color = color;
		}

		// Update and apply velocity.
		_velocity.x = runSpeed;
		_velocity.y -= GRAVITY * Time.deltaTime;
		if (IsPerformingLunge ()) {
			_velocity.y = 0.0f;
		}
		_controller.move( _velocity * Time.deltaTime );

		// Update animation states.
		if (_movementDirection == MovementDirectionState.FACING_LEFT) {
			_animator.SetBool ("isRunning", (_controlMoveState == ControlMoveState.MOVING && !CheckLeft(2.0f, _controller.platformMask)));
		} else {
			_animator.SetBool ("isRunning", (_controlMoveState == ControlMoveState.MOVING && !CheckRight(2.0f, _controller.platformMask)));
		}

		
		bool isSkidding = ((_movementDirection == MovementDirectionState.FACING_LEFT && _velocity.x > 0) ||
		                   (_movementDirection == MovementDirectionState.FACING_RIGHT && _velocity.x < 0));
		bool isSliding = (_velocity.x != 0 && _controlMoveState == ControlMoveState.NEUTRAL);

		_animator.SetBool ("isSkidding", isSkidding || isSliding);
		_animator.SetBool ("isGrounded", _controller.isGrounded);
		_animator.SetBool ("isPlunging", IsPerformingPlunge());
		_animator.SetBool ("isLunging", IsPerformingLunge());
		_animator.SetBool ("isParrying", _moveModState == MoveModState.PARRY_AOE);
		_animator.SetBool ("isNeutral", _moveModState == MoveModState.NEUTRAL);
		_animator.SetFloat ("velocityY", _velocity.y);
	}
	
	private void OnDestroy()
	{
		OnPlayerCreate = null;
		PlayerKillEvent = null;
		PlayerDeathEvent = null;
		PlayerRespawnEvent = null;
		PlayerEliminatedEvent = null;
		PlayerRecoilEvent = null;
		PlayerStunEvent = null;
		PlayerParryEvent = null;
		PlayerLandPlungeEvent = null;

		Resources.UnloadUnusedAssets();
	}
	
	// Setup.
	protected void AcceptInputEvents()
	{
		_inputController.OnMoveRightEvent += MoveRightEvent;
		_inputController.OnMoveLeftEvent  += MoveLeftEvent;
		_inputController.OnNoMoveEvent	  += NoMoveEvent;
		_inputController.OnJumpEvent	  += JumpEvent;
		_inputController.OnAttackEvent    += AttackEvent;
		_inputController.OnPlungeEvent    += PlungeEvent;
		_inputController.OnParryEvent     += ParryEvent;
	}
	
	protected void ClearInputEvents()
	{
		_inputController.OnMoveRightEvent -= MoveRightEvent;
		_inputController.OnMoveLeftEvent  -= MoveLeftEvent;
		_inputController.OnNoMoveEvent	  -= NoMoveEvent;
		_inputController.OnJumpEvent	  -= JumpEvent;
		_inputController.OnAttackEvent    -= AttackEvent;
		_inputController.OnPlungeEvent    -= PlungeEvent;
		_inputController.OnParryEvent     -= ParryEvent;
	}
	
	private void GetComponentReferences()
	{
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _inputController = GetComponent<ZMPlayerInputController>();
    }

	private void HandleOnMatchStart()
	{
		_animator.SetBool("didBecomeActive", true);

		EnablePlayer();
		AcceptInputEvents();
	}
	
	private void HandleOnMatchPause()
	{
		DisablePlayer();
		
		ClearInputEvents();
	}
	
	private void HandleOnMatchResume()
	{
		EnablePlayer();
		
		AcceptInputEvents();
	}
	
	private void WallJumpCooldown()
	{
		_canWallJump = true;
	}
	
	private void LungeCooldown()
	{
		_canLunge = true;
		_controlModState = ControlModState.NEUTRAL;
	}
	
	// Event handling - CCONTROL
	private void MoveRightEvent()
	{
		if (enabled)
		{
			_controlMoveState = ControlMoveState.MOVING;

			if (_movementDirection == MovementDirectionState.FACING_LEFT) { CheckSkidding (); }

			if (!IsPerformingLunge()) { SetMovementDirection(MovementDirectionState.FACING_RIGHT); }
		}
	}

	private void MoveLeftEvent()
	{
		if (enabled)
		{
			_controlMoveState = ControlMoveState.MOVING;

			if (_movementDirection == MovementDirectionState.FACING_RIGHT) { CheckSkidding (); }

			if (!IsPerformingLunge()) { SetMovementDirection(MovementDirectionState.FACING_LEFT); }
		}
	}

	private void NoMoveEvent()
	{
		_controlMoveState = ControlMoveState.NEUTRAL;
	}

	private void JumpEvent()
	{
		if (_controller.isGrounded) 
		{
			if (_controlModState != ControlModState.JUMPING) { _controlModState = ControlModState.JUMPING; }
		}
		else if (IsTouchingEitherSide() && _canWallJump) { _controlModState = ControlModState.WALL_JUMPING; }
	}

	private void AttackEvent(int direction)
	{
		if (!IsAttacking() && _moveModState != MoveModState.RESPAWN)
		{
			var forward = new Vector2(direction, 0);
			RaycastHit2D hit;

			if (_controller.isGrounded) { _controlModState = ControlModState.ATTACK; }
			else { _controlModState = ControlModState.AIR_ATTACK; }

			if (direction != 0)
			{
				SetMovementDirection(direction == -1 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);
			}

			// hack for destroying a breakable when pressed up against it
			if (_movementDirection == MovementDirectionState.FACING_LEFT) { hit = CheckLeft(10.0f, _controller.specialInteractibleMask); }
			else { hit = CheckRight(10.0f, _controller.specialInteractibleMask); }

			if (hit && Mathf.Round(Vector3.Dot(hit.normal, forward)) != 0 && hit.collider != null)
			{
				if (hit.collider.CompareTag("Breakable"))
				{
					hit.collider.GetComponent<ZMBreakable>().HandleCollision(_playerInfo);
				}
			}
		}
	}

	private void PlungeEvent()
	{
		if (!IsAttacking() && _moveModState != MoveModState.RESPAWN)
		{
			if (!_controller.isGrounded)
			{
				_controlModState = ControlModState.PLUNGE;
			}
			else
			{
				var hit = CheckBelow(2, _controller.specialInteractibleMask);

				if (hit && hit.collider != null)
				{
					if (hit.collider.CompareTag("Breakable"))
					{
						_controlModState = ControlModState.PLUNGE;
						hit.collider.GetComponent<ZMBreakable>().HandleCollision(_playerInfo);
					}
				}
			}
		}
	}

	private void ParryEvent()
	{
		if (!IsParrying () && _moveModState != MoveModState.RESPAWN)
		{
			_controlModState = ControlModState.PARRY;
		}
	}

	private void HorizontalAttack()
	{
		// Attack
		GetComponent<AudioSource>().PlayOneShot(_audioLunge[Random.Range (0, _audioLunge.Length)]);
		
		_controlModState = ControlModState.ATTACKING;
		_moveModState = MoveModState.LUNGE;
		
		var rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 180.0f : 0.0f), 0.0f));
		Instantiate(_effectLungeObject, new Vector2 (transform.position.x - 3, transform.position.y - 10), rotation);
	}

	void ResetControlModState() { 
		_controlModState = ControlModState.NEUTRAL;
	}

	void ResetMoveModState() { _moveModState = MoveModState.NEUTRAL; }

	void onControllerCollider( RaycastHit2D hit )
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")) {
			if (Mathf.Abs(hit.normal.x) > 0)
			if (_moveModState == MoveModState.RESPAWN) {
				if (Mathf.Abs(runSpeed) > FRICTION) {
					runSpeed *= -0.9f;
					GetComponent<AudioSource>().PlayOneShot(_audioBash[Random.Range(0, _audioBash.Length)], runSpeed / RUN_SPEED_MAX);
				}
			}
		}

		if (hit.collider.gameObject.layer == LayerMask.NameToLayer(kSpecialInteractiblesLayerMaskName)) {
			if (hit.normal.y == 1.0f) {
				if (hit.collider.CompareTag(kPlayerTag)) {
					ZMPlayerController otherPlayer = hit.collider.GetComponent<ZMPlayerController>();

					if (IsPerformingPlunge()) {
						KillOpponent (otherPlayer);
					}
				} else if (hit.collider.CompareTag("Breakable")) {
					if (IsPerformingPlunge()) {
						hit.collider.GetComponent<ZMBreakable>().HandleCollision(_playerInfo);
					}
				}
			}

			if (hit.normal.x == -1.0f || hit.normal.x == 1.0f) {
				// See if we hit a player.
				if (IsPerformingLunge()) {
					if (hit.collider.CompareTag(kPlayerTag)) {
						ZMPlayerController otherPlayer = hit.collider.GetComponent<ZMPlayerController>();

						if (_playerInPath && otherPlayer._playerInPath) {
							if (_movementDirection != otherPlayer._movementDirection) {
								CancelInvoke(kMethodNameEndLunge);
								_moveModState = MoveModState.RECOIL;
							}
						} else if (otherPlayer._moveModState == MoveModState.PARRY_FACING && IsOpposingDirection(otherPlayer)) {
							if (otherPlayer._canStun) {
								_moveModState = MoveModState.STUN;
								GetComponent<AudioSource>().PlayOneShot(_audioRecoil);
							} else {
								_moveModState = MoveModState.RECOIL;
								GetComponent<AudioSource>().PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
							}
						} else if (otherPlayer._moveModState == MoveModState.PARRY_AOE) {
							_moveModState = MoveModState.RECOIL;
						} else {
							_playerInPath = false;
							KillOpponent (otherPlayer);
						}
					} else if (hit.collider.CompareTag("Breakable")) {
						hit.collider.GetComponent<ZMBreakable>().HandleCollision(_playerInfo);
					}
				}
			}

			// Check for collision with volume.
			if (hit.collider.CompareTag(kWarpVolumeTag)) {
				GetComponent<ZMWarpController>().OnTriggerEnterCC2D(hit.collider);
			}
		}
	}
	
	void onTriggerEnterEvent( Collider2D collider ) {
		if (collider.CompareTag ("Grass")) {
			collider.GetComponent<ZMGrassController>().GrassEnter();
			if (IsPerformingLunge() || IsPerformingPlunge()) {
				collider.GetComponent<ZMGrassController>().CutGrass(_playerInfo);
			}
		}
	}
	
	void onTriggerExitEvent( Collider2D collider )
	{
		if (collider.CompareTag ("Grass")) {
			collider.GetComponent<ZMGrassController>().GrassExit();
		}
	}

	void HandleMinScoreReached(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			gameObject.SetActive(false);

			_moveModState = MoveModState.ELIMINATED;

			Notifier.SendEventNotification(PlayerEliminatedEvent, this);
		}
	}

	public void Respawn(Vector3 position)
	{
		Reset();
		
		transform.position = position;
		
		Notifier.SendEventNotification(PlayerRespawnEvent, this);
	}

	// Player state utility methods
	protected void EnablePlayer()
	{
		if (_moveModState == MoveModState.ELIMINATED) return;

		_moveModState = MoveModState.NEUTRAL;

		_controller.enabled = true;
		enabled = true;

		_animator.enabled = true;
	}

	protected void DisablePlayer()
	{
		_animator.enabled = false;
		_controller.enabled = false;
		enabled = false;
	}

	private void SetMovementDirection(MovementDirectionState direction)
	{
		var previousDirection = _movementDirection;
		var angle = (direction == MovementDirectionState.FACING_LEFT ? 180 : 0);

		_movementDirection = direction;

		transform.localRotation = Quaternion.Euler(0, angle, 0);

		if (!previousDirection.Equals(_movementDirection)) {
			Vector3 shiftedPos = transform.position;

			if (CheckRight(2.0f, _controller.platformMask)) {
				shiftedPos.x -= 4.0f;
			} else if (CheckLeft(2.0f, _controller.platformMask)) {
				shiftedPos.x += 4.0f;
			}

			transform.position = shiftedPos;
		}
	}

	private void CheckSkidding() {
		if (_controller.isGrounded) {
			int direction = (_movementDirection == MovementDirectionState.FACING_RIGHT ? 1 : -1);
			Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 0.0f : 180.0f), 0.0f));
			Instantiate (_effectSkidObject, new Vector2 (transform.position.x + 30 * direction, transform.position.y - 20), rotation);
		}
	}

	private void AddVelocity(Vector2 velocity) {
		runSpeed = velocity.x;
		_velocity.y = velocity.y;
	}

	private void KillSelf(ZMPlayerController killer)
	{
		GameObject body;

		_moveModState = MoveModState.RESPAWN;
		_controlModState = ControlModState.NEUTRAL;
		_controlMoveState = ControlMoveState.NEUTRAL;

		Notifier.SendEventNotification(PlayerDeathEvent, _playerInfo);

		CancelInvoke(kMethodNameEndLunge);
		CancelInvoke("ResetControlModState");
		CancelInvoke(kMethodNameEnablePlayer);

		// Handle death visuals here
		this.GetComponent<Renderer>().material.color = Color.red;
		GetComponent<Light>().enabled = false;
		_spriteRenderer.enabled = false;

		// load and instantiate the body's upper half
		body = GameObject.Instantiate(_upperBodyTemplate) as GameObject;
		body.transform.position = transform.position;

		ZMAddForce upperBody = body.GetComponent<ZMAddForce>();
		upperBody.ParticleColor = GetComponent<Light>().color;
		upperBody.AddForce(new Vector2(killer.runSpeed / 12, 0));

		// load and instantiate the body's lower half
		body = GameObject.Instantiate(_lowerBodyTemplate) as GameObject;
		body.transform.position = transform.position;

		ZMAddForce lowerBody = body.GetComponent<ZMAddForce>();
		lowerBody.ParticleColor = GetComponent<Light>().color;
		lowerBody.AddForce(new Vector2(killer.runSpeed / 12, 0));

		// Set player states
		_playerInPath = false;

		ClearInputEvents();

		GetComponent<AudioSource>().PlayOneShot(_audioGore[Random.Range (0, _audioGore.Length)]);
		GetComponent<AudioSource>().PlayOneShot(_audioHurt[Random.Range (0, _audioHurt.Length)]);
		GetComponent<AudioSource>().PlayOneShot(_audioKill[Random.Range (0, _audioKill.Length)]);
		_goreEmitter.Play();

		// Handle taunt text.
		if (_tauntText)
		{
			_tauntText.gameObject.SetActive (true);
			_tauntText.text = kDeathStrings [Random.Range (0, kDeathStrings.Length)];
			_tauntText.transform.rotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, Random.Range (-20, 20)));
			_tauntText.transform.position = new Vector3 (Random.Range (-100, 100), Random.Range (-100, 100), 0.0f);

			StartCoroutine (ScaleTauntText (new Vector3 (1.5f, 1.5f, 1.5f), Vector3.one, 0.05f));
			Invoke ("HideTauntText", 0.5f);
		}
	}

	private void KillOpponent(ZMPlayerController playerController) 
	{
		if (playerController.IsAbleToDie())
		{
			playerController.KillSelf(this);

			Notifier.SendEventNotification(PlayerKillEvent, this);

			// add the stat
			ZMStatTracker.Instance.Kills.Add(_playerInfo);
		}
	}

	private void DisableInputWithCallbackDelay(float delay)
	{
		ClearInputEvents();
        Invoke ("AcceptInputEvents", delay);
	}

	private IEnumerator ScaleTauntText(Vector3 start, Vector3 end, float totalTime) {
		float t = 0;
		while (t < totalTime) {
			_tauntText.transform.localScale = Vector3.Lerp(start, end, t / totalTime);
			yield return null;
			t += Time.deltaTime;
		} 
		_tauntText.transform.localScale = end;
		yield break;
	}

	private void HideTauntText() {
		_tauntText.gameObject.SetActive (false);
	}

	private void Reset()
	{
		_controlModState = ControlModState.NEUTRAL;
		_controlMoveState = ControlMoveState.NEUTRAL;
		runSpeed = 0.0f;

		_spriteRenderer.enabled = true;
		GetComponent<Light>().enabled = true;

		EnablePlayer();
		SetMovementDirection(transform.position.x < 0 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);

		if (!MatchStateManager.IsPause() && !MatchStateManager.IsEnd()) { AcceptInputEvents(); }
	}

	private void Recoil()
	{
		float recoilSpeed = 700f;
		_velocity.y = JUMP_HEIGHT / 2.0f;

		if (_movementDirection == MovementDirectionState.FACING_LEFT) {
			runSpeed = recoilSpeed;
		} else if (_movementDirection == MovementDirectionState.FACING_RIGHT) {
			runSpeed = -recoilSpeed;
		}
	}


	private void Plunge() {
		runSpeed = 0.0f;
		_velocity.y = -PLUNGE_SPEED;
	}

	private void LungeRight() {
		runSpeed = LUNGE_SPEED;
	}

	private void LungeLeft() {
		runSpeed = -LUNGE_SPEED;
	}

	private void EndLunge() {
		runSpeed = 0;
		_playerInPath = false;

		float lagTime = (_moveModState == MoveModState.LUNGING_AIR ? PARRY_TIME_AIR : PARRY_TIME_LUNGE);
		_moveModState = MoveModState.PARRY_FACING;
		_controlModState = ControlModState.NEUTRAL;
		Invoke(kMethodNameEnablePlayer, lagTime);

		// Set a cooldown before we can lunge again.
		Invoke ("LungeCooldown", lagTime);
		_canLunge = false;
		DisablePlayer();
	}

	private void EndParry() {
		_moveModState = MoveModState.NEUTRAL;
		_controlModState = ControlModState.NEUTRAL;
		_canLunge = true;
		this.GetComponent<Light>().color = _baseColor;
	}

	private void EndStunBeginParry() {
		_canStun = false;
		GetComponent<Renderer>().material = _materialDefault;
		Invoke(kEndParryMethodName, PARRY_TIME);
	}

	private void EndStun() {
		_canStun = false;
		GetComponent<Renderer>().material = _materialDefault;
		EndParry();
	}

	private RaycastHit2D CheckLeft(float distance, LayerMask mask) {
		return CheckLeft(Vector2.zero, distance, mask);
	}

	private RaycastHit2D CheckLeft(Vector2 offset, float distance, LayerMask mask) {
		Vector2 rayOrigin = new Vector2(transform.position.x - 17f, transform.position.y);
		Vector2 rayDirection = new Vector2(-1.0f, 0.0f);

		rayOrigin += offset;
		
		return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
	}

	private RaycastHit2D CheckRight(float distance, LayerMask mask) {
		return CheckRight (Vector2.zero, distance, mask);
	}

	private RaycastHit2D CheckRight(Vector2 offset, float distance, LayerMask mask) {
		Vector2 rayOrigin = new Vector2(transform.position.x + 17f, transform.position.y);
		Vector2 rayDirection = new Vector2(1.0f, 0.0f);

		rayOrigin += offset;
		
		return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
	}

	private RaycastHit2D CheckBelow(float distance, LayerMask mask) {
		return CheckBelow(Vector2.zero, distance, mask);
	}

	private RaycastHit2D CheckBelow(Vector2 offset, float distance, LayerMask mask) {
		Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 32.1f);
		Vector2 rayDirection = new Vector2(1.0f, 0.0f);
		
		rayOrigin += offset;
		Debug.DrawRay(rayOrigin, rayDirection, Color.yellow);
		
		return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
	}

	private bool IsTouchingEitherSide() {
		return _checkTouchingLeft || _checkTouchingRight;
	}

	private bool IsMovingLeft() {
		return _controlMoveState == ControlMoveState.MOVING && _movementDirection == MovementDirectionState.FACING_LEFT;
	}

	private bool IsMovingRight() {
		return _controlMoveState == ControlMoveState.MOVING && _movementDirection == MovementDirectionState.FACING_RIGHT;
	}

	private bool IsTouchingLeftAndMovingLeft() {
		return _checkTouchingLeft &&  runSpeed < 0;
	}

	private bool IsTouchingRightAndMovingRight() {
		return _checkTouchingRight && runSpeed > 0;
	}

	private bool IsPerformingPlunge() {
		return _moveModState == MoveModState.PLUNGE || _moveModState == MoveModState.PLUNGING;
	}

	private bool IsPerformingLunge() {
		return _moveModState == MoveModState.LUNGE || _moveModState == MoveModState.LUNGING_GROUND || _moveModState == MoveModState.LUNGING_AIR;
	}

	private bool IsRecoiling() {
		return _moveModState == MoveModState.RECOIL || _moveModState == MoveModState.RECOILING;
	}

	private bool IsParrying() { 
		return _moveModState == MoveModState.PARRY_FACING || _moveModState == MoveModState.PARRY_AOE; 
	}

	private bool IsAbleToKill() {
		return IsPerformingLunge() || IsPerformingPlunge();
	}

	private bool IsAbleToDie() {
		return _moveModState != MoveModState.RESPAWN && !IsRecoiling();
	}

	public bool IsDead() {
		return _moveModState == MoveModState.RESPAWN;
	}

	private bool IsOpposingDirection(ZMPlayerController otherPlayer) {
		return _movementDirection != otherPlayer._movementDirection;
	}

	private bool ShouldDisableWhenGrounded() {
		return _moveModState == MoveModState.DISABLE && _controller.isGrounded;
	}

	private bool ShouldEnable() {
		return (!_controller.isGrounded && !IsPerformingPlunge () && !IsRecoiling () 
			&& !IsPerformingLunge () && _moveModState != MoveModState.PARRY_AOE && _moveModState != MoveModState.STUNNED);
	}

	private bool ShouldRecoilWithPlayer(ZMPlayerController other) {
		return IsPerformingLunge() && other.IsPerformingLunge();
	}

	private bool IsAttacking() {
		return  _controlModState == ControlModState.ATTACKING || _controlModState == ControlModState.ATTACK;
	}
}
