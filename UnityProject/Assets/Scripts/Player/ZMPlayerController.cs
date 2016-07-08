using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ZMPlayer;
using Core;
using ZMConfiguration;

[RequireComponent(typeof(ZMPlayerInputController))]
[RequireComponent(typeof(BoxCollider2D))]
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
	private ZMPlayerInputEventNotifier _inputEventNotifier;

	// Unity component references.
	private Light _light;
	private AudioSource _audio;

	private RaycastHit2D _lastControllerColliderHit;
	private RaycastHit2D _checkTouchingLeft;
	private RaycastHit2D _checkTouchingRight;

	private Vector3 _velocity;
	private Vector3 _storedVelocity;

	private int _framesUntilStep = 0;
	private bool _canLunge;
	private bool _canWallJump;
	private bool _canAirLunge;

	// Speeds of two players before recoil.
	private Vector2 _collisionVelocities;

	// Player states.
	private enum MovementDirectionState { FACING_LEFT = -1, FACING_RIGHT = 1 };
	private enum ControlMoveState 		{ NEUTRAL, MOVING };
	private enum ControlModState	    { NEUTRAL, JUMPING, ATTACK, AIR_ATTACK, ATTACKING, WALL_JUMPING, PLUNGE, PLUNGING, PARRY, PARRYING };
	private enum MoveModState 		    { NEUTRAL, PLUNGE, PLUNGING, LUNGE, LUNGING_AIR, LUNGING_GROUND, WALL_SLIDE, RECOIL, RECOILING, STUN, STUNNED, DISABLE, DISABLED, PARRY_FACING, PARRY_AOE, RESPAWN, ELIMINATED };
	private enum AbilityState 			{ NEUTRAL, SHOOTING };

	private ControlMoveState _controlMoveState;
	private ControlModState _controlModState;
	private MovementDirectionState _movementDirection;
	private MoveModState _moveModState;

	// Flags.
	private bool _playerInPath;
	private bool _canStun;

	// Coroutines.
	private Coroutine _enablePlayerCoroutine;
	private Coroutine _resetControlModCoroutine;
	private Coroutine _endLungeCoroutine;

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
	public SpriteRenderer _fadeEffect;

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
	private GameObject _lowerBodyTemplate;
	private GameObject _upperBodyTemplate;

	// Materials
	private Material _material;
	public Material _materialFlash;

	// Delegates
	public static EventHandler<ZMPlayerInfo> OnPlayerCreate;
	public static EventHandler<ZMPlayerController> OnPlayerKill;
	public static EventHandler<ZMPlayerInfo> OnPlayerDeath;
	public static EventHandler<ZMPlayerController> OnPlayerRespawn;
	public static EventHandler<ZMPlayerController> OnPlayerEliminated;
	public static EventHandler<ZMPlayerController, float> OnPlayerRecoil;
	public static EventHandler<ZMPlayerController, float> OnPlayerStun;
	public static EventHandler<ZMPlayerController, float> OnPlayerParry;
	public static EventHandler OnPlayerLandPlunge;

	// Constants
	private const float EDGE_OFFSET = 16.0f;
	private const string kBodyUpperHalfPath = "PlayerHalfUpper";
	private const string kBodyLowerHalfPath = "PlayerHalfLower";

	private readonly Vector2 LEFT_EDGE_OFFSET = new Vector2(-EDGE_OFFSET - 1, 0.0f);
	private readonly Vector2 RIGHT_EDGE_OFFSET = new Vector2(EDGE_OFFSET + 1, 0.0f);

	// Debug
	private SpriteRenderer _spriteRenderer;
	private Color _baseColor;

	private Vector3 _posPrevious;
	private int _fadeSpawnCycleLen;
	private int _fadeSpawnIndex;

	protected override void Awake()
	{
		base.Awake();

		_moveModState = MoveModState.NEUTRAL;
		_fadeSpawnCycleLen = 2;

		GetComponentReferences();

		_canLunge = true;
		_canWallJump = true;
		_canAirLunge = true;

		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		_fadeEffect.color = _spriteRenderer.color;

		ZMStageScoreController.OnReachMinScore += HandleMinScoreReached;

		MatchStateManager.OnMatchPause += HandleOnMatchPause;
		MatchStateManager.OnMatchResume += HandleOnMatchResume;
		MatchStateManager.OnMatchStart += HandleOnMatchStart;
		MatchStateManager.OnMatchEnd += HandleOnMatchPause;

		ZMPlayerInputRecorder.OnPlaybackBegin += HandlePlaybackBegin;

		// load resources
		_upperBodyTemplate = Resources.Load(kBodyUpperHalfPath, typeof(GameObject)) as GameObject;
		_lowerBodyTemplate = Resources.Load(kBodyLowerHalfPath, typeof(GameObject)) as GameObject;
	}

	protected virtual void Start()
	{
		// Set original facing direction so the player faces the center of the stage.
		SetMovementDirection(transform.position.x > 0 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);

		_baseColor = _light.color;
		_goreEmitter.GetComponent<Renderer>().material.color = _baseColor;
		_goreEmitter.startColor = _baseColor;

		_material.color = Color.black;

		Notifier.SendEventNotification(OnPlayerCreate, _playerInfo);
	}

	void Update()
	{
		// Set raycasts. These will check one tile unit to the left and right to help with things like wall jump.
		_checkTouchingLeft  = Environment.CheckLeft(transform.position, LEFT_EDGE_OFFSET, TILE_SIZE, _controller.platformMask);
		_checkTouchingRight = Environment.CheckRight(transform.position, RIGHT_EDGE_OFFSET, TILE_SIZE, _controller.platformMask);

		// Update the velocity calculated from the controller.
		_velocity = _controller.velocity;
		
		// Check raycasts.
		if (_controller.isGrounded)
		{
			// Landing.
			if (_velocity.y < -1) {
				_audio.PlayOneShot(_audioLand[Random.Range (0, _audioLand.Length)], 0.5f);
				Instantiate(_effectLandObject, new Vector2(transform.position.x - 3, transform.position.y - 8), transform.rotation);
			}

			_velocity.y = 0;

			if (IsPlunging()) {
				_audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);

				Notifier.SendEventNotification(OnPlayerLandPlunge);
			}
		}

		// Horizontal movement. Only want to move if we aren't doing some kind of attack
		// or getting knocked back (recoiling).
		if (_controlMoveState == ControlMoveState.MOVING && !IsPlunging() && !IsRecoiling())
		{
			if (_movementDirection == MovementDirectionState.FACING_RIGHT)
			{
				if (runSpeed < RUN_SPEED_MAX)
				{
					runSpeed += ACCELERATION;
				}
			}
			else if (_movementDirection == MovementDirectionState.FACING_LEFT)
			{
				if (runSpeed > -RUN_SPEED_MAX)
				{
					runSpeed -= ACCELERATION;
				}
			}

			if (_controller.isGrounded && Mathf.Abs(_velocity.x) > ACCELERATION) {
				_framesUntilStep++;
				if (_framesUntilStep >= FRAMES_PER_STEP) {
					_framesUntilStep = 0;
					_audio.PlayOneShot(_audioStep[Random.Range (0, _audioStep.Length)], 0.25f);
				}
			}
		}

		// Friction.
		if (Mathf.Abs(runSpeed) <= FRICTION)
		{
			runSpeed = 0.0f;
		}
		else
		{
			if (_controlMoveState == ControlMoveState.NEUTRAL)
			{
				runSpeed -= FRICTION * Mathf.Sign(runSpeed);
			}
		}

		// Jumping.
		if (_controlModState == ControlModState.JUMPING)
		{
			_controlModState = ControlModState.NEUTRAL;

			_velocity.y = JUMP_HEIGHT;

			_audio.PlayOneShot(_audioJump[Random.Range (0, _audioJump.Length)]);
			Instantiate(_effectJumpObject, new Vector2(transform.position.x - 3, transform.position.y - 8), transform.rotation);
		}

		// Update movement and ability state.
		if (_controlModState == ControlModState.ATTACK)
		{
			if (!IsLunging() && _canLunge)
			{
				HorizontalAttack();
			}
			else
			{
				_audio.PlayOneShot(_audioLungeFail);
			}
		}
		else if (_controlModState == ControlModState.AIR_ATTACK)
		{
			if (!IsLunging() && _canLunge && _canAirLunge)
			{
				_canAirLunge = false;
				HorizontalAttack();
			}
			else
			{
				_audio.PlayOneShot(_audioLungeFail);
			}
		}
		else if (_controlModState == ControlModState.PLUNGE)
		{
			_controlModState = ControlModState.PLUNGING;

			if (!IsPlunging())
			{
				_audio.PlayOneShot(_audioPlunge[Random.Range (0, _audioPlunge.Length)]);
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

			_material = _materialFlash;
			_audio.PlayOneShot(_audioParry);

			if (_controller.isGrounded)
			{
				Utilities.ExecuteAfterDelay(EndStunBeginParry, PARRY_STUN_WINDOW);
				DisableInputWithCallbackDelay(PARRY_STUN_WINDOW + PARRY_TIME);

				Notifier.SendEventNotification(OnPlayerParry, this, PARRY_STUN_WINDOW + PARRY_TIME);
			}
		}
		else if (_controlModState == ControlModState.NEUTRAL)
		{
			if (_controller.isGrounded)
			{
				_canAirLunge = true;
			}

			if (IsTouchingEitherSide())
			{
				if (!_controller.isGrounded && _moveModState == MoveModState.NEUTRAL)
				{
					_moveModState = MoveModState.WALL_SLIDE;
				}
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
				ZMPlayerController playerController;
				var recoilOffsetRight = new Vector2(EDGE_OFFSET, -16.0f);
				var recoilOffsetLeft = new Vector2(-EDGE_OFFSET, -16.0f);
				var recoilAOE = Environment.CheckRight(transform.position, recoilOffsetRight, AOE_RANGE, _controller.specialInteractibleMask);

				Instantiate(_effectPlungeObject, new Vector2(transform.position.x, transform.position.y), transform.rotation);

				// Check right.
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
				recoilAOE = Environment.CheckLeft(transform.position, recoilOffsetLeft, AOE_RANGE, _controller.specialInteractibleMask);

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

				_enablePlayerCoroutine = Utilities.ExecuteAfterDelay(EnablePlayer, PARRY_TIME_LUNGE);
				_resetControlModCoroutine = Utilities.ExecuteAfterDelay(ResetControlModState, PARRY_TIME_LUNGE + 0.02f);
			}
		}
		else if (_moveModState == MoveModState.LUNGE)
		{
			_moveModState = _controller.isGrounded ? MoveModState.LUNGING_GROUND : MoveModState.LUNGING_AIR;

			RaycastHit2D checkPlayer;

			if (_movementDirection == MovementDirectionState.FACING_RIGHT)
			{
				if (checkPlayer = Environment.CheckRight(transform.position, RIGHT_EDGE_OFFSET, 145f, _controller.specialInteractibleMask))
				{
					if (checkPlayer.collider.CompareTag(Tags.kPlayerTag) && !_playerInPath)
					{
						_playerInPath = true;
					}
				}

				LungeRight();
			}
			else if (_movementDirection == MovementDirectionState.FACING_LEFT)
			{
				if (checkPlayer = Environment.CheckLeft(transform.position, LEFT_EDGE_OFFSET, 145f, _controller.specialInteractibleMask))
				{
					if (checkPlayer.collider.CompareTag(Tags.kPlayerTag) && !_playerInPath)
					{
						_playerInPath = true;
					}
				}

				LungeLeft();
			}

			// End the lunge after a delay so we don't zoom across the map forever.
			Utilities.StopDelayRoutine(_endLungeCoroutine);
			_endLungeCoroutine = Utilities.ExecuteAfterDelay(EndLunge, LUNGE_TIME);
		}

		if (_movementDirection == MovementDirectionState.FACING_RIGHT &&
			(_moveModState == MoveModState.LUNGING_AIR || _moveModState == MoveModState.LUNGING_GROUND))
		{
			var hit = Environment.CheckRight(transform.position, RIGHT_EDGE_OFFSET, 4, _controller.platformMask);

			if (hit && !hit.collider.CompareTag("Breakable"))
			{
				_audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
				Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 90.0f));
				Instantiate(_effectPlungeObject, new Vector2(transform.position.x - 18, transform.position.y), rotation);
				EndLunge ();
			}
		}
		else if (_movementDirection == MovementDirectionState.FACING_LEFT &&
			(_moveModState == MoveModState.LUNGING_AIR || _moveModState == MoveModState.LUNGING_GROUND))
		{
			var hit = Environment.CheckLeft(transform.position, LEFT_EDGE_OFFSET, 4, _controller.platformMask);

			if (hit && !hit.collider.CompareTag("Breakable"))
			{
				_audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
				Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, 270.0f));
				Instantiate(_effectPlungeObject, new Vector2(transform.position.x + 14, transform.position.y), rotation);
				EndLunge ();
			}
		}

		if (_moveModState == MoveModState.RECOIL)
		{
			_audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
			Instantiate(_effectClashObject, new Vector2(transform.position.x, transform.position.y), transform.rotation);
			_moveModState = MoveModState.RECOILING;

			Utilities.StopDelayRoutine(_endLungeCoroutine);

			Recoil();

			DisableInputWithCallbackDelay(RECOIL_STUN_TIME);

			Notifier.SendEventNotification(OnPlayerRecoil, this, RECOIL_STUN_TIME);
		}
		else if (_moveModState == MoveModState.RECOILING)
		{
			_moveModState = MoveModState.NEUTRAL;
			_controlModState = ControlModState.NEUTRAL;
			_controlMoveState = ControlMoveState.NEUTRAL;

			_playerInPath = false;
		}
		else if (_moveModState == MoveModState.STUN)
		{
			_moveModState = MoveModState.STUNNED;
			_controlModState = ControlModState.NEUTRAL;
			_controlMoveState = ControlMoveState.NEUTRAL;
			_audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);

			Utilities.StopDelayRoutine(_endLungeCoroutine);

			Recoil();
			Utilities.ExecuteAfterDelay(ResetMoveModState, STUN_TIME);
			DisableInputWithCallbackDelay(STUN_TIME);

			Notifier.SendEventNotification(OnPlayerStun, this, STUN_TIME);
		}
		else if (_moveModState == MoveModState.WALL_SLIDE)
		{
			// Wall slide.
			if (_velocity.y < 1.0f &&  _controlMoveState == ControlMoveState.MOVING)
			{
				_velocity.y = -WALL_SLIDE_SPEED;
				if (IsTouchingRightAndMovingRight() || IsTouchingLeftAndMovingLeft())
				{
					runSpeed = 0;
				}
			}
			if (_controller.isGrounded || !IsTouchingEitherSide())
			{
				_moveModState = MoveModState.NEUTRAL;
			}
			// Wall jump.
			if (_controlModState == ControlModState.WALL_JUMPING)
			{
				_controlModState = ControlModState.NEUTRAL;

				if (IsMovingLeft () || IsMovingRight())
				{
					if (!_controller.isGrounded)
					{
						var rotation = Quaternion.Euler(new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 180.0f : 0.0f), 0.0f));
						var skidOffset = _movementDirection == MovementDirectionState.FACING_RIGHT ? 12.0f : -12.0f;

						_velocity.y = JUMP_HEIGHT;
						_audio.PlayOneShot(_audioJump[Random.Range (0, _audioJump.Length)]);

						Instantiate (_effectSkidObject, new Vector2 (transform.position.x + skidOffset, transform.position.y - 20), rotation);
						_moveModState = MoveModState.NEUTRAL;
						_canWallJump = false;
						_canAirLunge = true;
						Utilities.ExecuteAfterDelay(WallJumpCooldown, 0.05f);

						if (IsMovingLeft())
						{
							runSpeed = WALL_JUMP_KICK_SPEED * 0.6f;
						}
						else if (IsMovingRight())
						{
							runSpeed = -WALL_JUMP_KICK_SPEED * 0.6f;
						}
					}
				}
			}
		}
		else if (IsAttacking() || IsPlunging())
		{
			var dist = 0.0f;
			var lerpPos = _posPrevious;
			var mult = 0.5f;

			if (_fadeSpawnIndex == _fadeSpawnCycleLen)
			{
				// ZVP
				var fadeObject = Instantiate(_fadeEffect, lerpPos, transform.rotation) as SpriteRenderer;

				lerpPos = Vector3.Lerp(lerpPos, transform.position, mult * Time.deltaTime);

				fadeObject.sprite = _spriteRenderer.sprite;

				dist += Vector3.Distance(transform.position, lerpPos);
				_fadeSpawnIndex = 0;
			}

			_fadeSpawnIndex += 1;
		}

		// Update visuals.
		if (!_controller.isGrounded && !_canAirLunge) {
			Color color = _material.color;
			color.a = 0.5f;
			_material.color = color;
		} else {
			Color color = _material.color;
			color.a = 1.0f;
			_material.color = color;
		}

		// Update and apply velocity.
		_velocity.x = runSpeed;
		_velocity.y -= GRAVITY * Time.deltaTime;

		// Don't want gravity to affect the player if lunging.
		if (IsLunging())
		{
			_velocity.y = 0.0f;
		}

		_posPrevious = transform.position;
		_controller.move(_velocity * Time.deltaTime);

		UpdateAnimator();
	}

	private void OnDestroy()
	{
		OnPlayerCreate = null;
		OnPlayerKill = null;
		OnPlayerDeath = null;
		OnPlayerRespawn = null;
		OnPlayerEliminated = null;
		OnPlayerRecoil = null;
		OnPlayerStun = null;
		OnPlayerParry = null;
		OnPlayerLandPlunge = null;

		Resources.UnloadUnusedAssets();
	}

	// Setup.
	protected void AcceptInputEvents()
	{
		_inputEventNotifier.OnMoveRightEvent += MoveRightEvent;
		_inputEventNotifier.OnMoveLeftEvent  += MoveLeftEvent;
		_inputEventNotifier.OnNoMoveEvent	 += NoMoveEvent;
		_inputEventNotifier.OnJumpEvent	  	 += JumpEvent;
		_inputEventNotifier.OnAttackEvent    += AttackEvent;
		_inputEventNotifier.OnPlungeEvent    += PlungeEvent;
		_inputEventNotifier.OnParryEvent     += ParryEvent;
	}
	
	protected void ClearInputEvents()
	{
		_inputEventNotifier.OnMoveRightEvent -= MoveRightEvent;
		_inputEventNotifier.OnMoveLeftEvent  -= MoveLeftEvent;
		_inputEventNotifier.OnNoMoveEvent	 -= NoMoveEvent;
		_inputEventNotifier.OnJumpEvent	  	 -= JumpEvent;
		_inputEventNotifier.OnAttackEvent    -= AttackEvent;
		_inputEventNotifier.OnPlungeEvent    -= PlungeEvent;
		_inputEventNotifier.OnParryEvent     -= ParryEvent;
	}
	
	private void GetComponentReferences()
	{		
		_animator = GetComponent<Animator>();
		_material = GetComponent<Renderer>().material;
		_controller = GetComponent<CharacterController2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
		_light = GetComponent<Light>();
		_audio = GetComponent<AudioSource>();

		SetInputNotifierToController();
    }

	private void SetInputNotifierToController()
	{
		var inputController = GetComponent<ZMPlayerInputController>();

		_inputEventNotifier = inputController._inputEventNotifier;
	}

	private void UpdateAnimator()
	{
		bool isChangingDirectionToRight = _movementDirection == MovementDirectionState.FACING_LEFT && _velocity.x > 0;
		bool isChangingDirectionToLeft = _movementDirection == MovementDirectionState.FACING_RIGHT && _velocity.x < 0;

		bool isSkidding = isChangingDirectionToRight || isChangingDirectionToLeft;
		bool isSliding = _velocity.x != 0 && _controlMoveState == ControlMoveState.NEUTRAL;
		bool isTouchingLeft = Environment.CheckLeft(transform.position, LEFT_EDGE_OFFSET, 2.0f, _controller.platformMask);
		bool isTouchingRight = Environment.CheckRight(transform.position, RIGHT_EDGE_OFFSET, 2.0f, _controller.platformMask);

		// Update animation states.
		if (_movementDirection == MovementDirectionState.FACING_LEFT)
		{
			_animator.SetBool ("isRunning", _controlMoveState == ControlMoveState.MOVING && !isTouchingLeft);
		}
		else
		{
			_animator.SetBool ("isRunning", _controlMoveState == ControlMoveState.MOVING && !isTouchingRight);
		}

		_animator.SetBool ("isSkidding", isSkidding || isSliding);
		_animator.SetBool ("isGrounded", _controller.isGrounded);
		_animator.SetBool ("isPlunging", IsPlunging());
		_animator.SetBool ("isLunging", IsLunging());
		_animator.SetBool ("isParrying", _moveModState == MoveModState.PARRY_AOE);
		_animator.SetBool ("isNeutral", _moveModState == MoveModState.NEUTRAL);
		_animator.SetFloat ("velocityY", _velocity.y);
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

	private void HandlePlaybackBegin(ZMPlayerInfo info, ZMPlayerInputRecorder recorder)
	{
		if (_playerInfo == info)
		{
			_inputEventNotifier = recorder._inputEventNotifier;
			AcceptInputEvents();
		}
	}

	private void HandlePlaybackEnd(ZMPlayerInfo info, ZMPlayerInputRecorder recorder)
	{
		if (_playerInfo == info)
		{
			SetInputNotifierToController();
		}
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
		if (IsAbleToReceiveInput())
		{
			_controlMoveState = ControlMoveState.MOVING;

			if (_movementDirection == MovementDirectionState.FACING_LEFT)
			{
				CheckSkidding();
			}

			if (!IsLunging())
			{
				SetMovementDirection(MovementDirectionState.FACING_RIGHT);
			}
		}
	}

	private void MoveLeftEvent()
	{
		if (IsAbleToReceiveInput())
		{
			_controlMoveState = ControlMoveState.MOVING;

			if (_movementDirection == MovementDirectionState.FACING_RIGHT) { CheckSkidding (); }

			if (!IsLunging()) { SetMovementDirection(MovementDirectionState.FACING_LEFT); }
		}
	}

	private void NoMoveEvent()
	{
		_controlMoveState = ControlMoveState.NEUTRAL;
	}

	private void JumpEvent()
	{
		if (IsAbleToReceiveInput())
		{
			if (_controller.isGrounded) 
			{
				if (_controlModState != ControlModState.JUMPING)
				{
					_controlModState = ControlModState.JUMPING;
				}
			}
			else if (IsTouchingEitherSide() && _canWallJump)
			{
				_controlModState = ControlModState.WALL_JUMPING;
			}
		}
	}

	private void AttackEvent(int direction)
	{
		if (!IsAttacking() && IsAbleToReceiveInput())
		{
			var forward = new Vector2(direction, 0);
			RaycastHit2D hit;

			if (_controller.isGrounded)
			{
				_controlModState = ControlModState.ATTACK;
			}
			else
			{
				_controlModState = ControlModState.AIR_ATTACK;
			}

			if (direction != 0)
			{
				SetMovementDirection(direction == -1 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);
			}

			// hack for destroying a breakable when pressed up against it
			if (_movementDirection == MovementDirectionState.FACING_LEFT)
			{
				hit = Environment.CheckLeft(transform.position, LEFT_EDGE_OFFSET, 10.0f, _controller.specialInteractibleMask);
			}
			else
			{
				hit = Environment.CheckRight(transform.position, RIGHT_EDGE_OFFSET, 10.0f, _controller.specialInteractibleMask);
			}

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
		if (!IsAttacking() && IsAbleToReceiveInput())
		{
			if (!_controller.isGrounded)
			{
				_controlModState = ControlModState.PLUNGE;
			}
			else
			{
				var offset = new Vector2(0, -32.1f);
				var hit = Environment.CheckBelow(transform.position, offset, 2, _controller.specialInteractibleMask);

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
		if (!IsParrying () && IsAbleToReceiveInput())
		{
			_controlModState = ControlModState.PARRY;
		}
	}

	private void HorizontalAttack()
	{
		// Attack
		_audio.PlayOneShot(_audioLunge[Random.Range (0, _audioLunge.Length)]);
		
		_controlModState = ControlModState.ATTACKING;
		_moveModState = MoveModState.LUNGE;
		
		var rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 180.0f : 0.0f), 0.0f));
		Instantiate(_effectLungeObject, new Vector2 (transform.position.x - 3, transform.position.y - 10), rotation);
	}

	private void ResetControlModState()
	{
		_controlModState = ControlModState.NEUTRAL;
	}

	private void ResetMoveModState()
	{
		_moveModState = MoveModState.NEUTRAL;
	}

	void onControllerCollider(RaycastHit2D hit)
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer(LayerMaskNames.kGroundLayerMaskName))
		{
			if (Mathf.Abs(hit.normal.x) > 0)
			{
				if (_moveModState == MoveModState.RESPAWN)
				{
					if (Mathf.Abs(runSpeed) > FRICTION)
					{
						runSpeed *= -0.9f;
						_audio.PlayOneShot(_audioBash[Random.Range(0, _audioBash.Length)], runSpeed / RUN_SPEED_MAX);
					}
				}
			}
		}

		if (hit.collider.gameObject.layer == LayerMask.NameToLayer(LayerMaskNames.kSpecialInteractiblesLayerMaskName))
		{
			if (hit.normal.y == 1.0f)
			{
				if (hit.collider.CompareTag(Tags.kPlayerTag))
				{
					ZMPlayerController otherPlayer = hit.collider.GetComponent<ZMPlayerController>();

					if (IsPlunging())
					{
						KillOpponent (otherPlayer);
					}
				}
				else if (hit.collider.CompareTag(Tags.kBreakable))
				{
					if (IsPlunging())
					{
						hit.collider.GetComponent<ZMBreakable>().HandleCollision(_playerInfo);
					}
				}
			}

			if (hit.normal.x == -1.0f || hit.normal.x == 1.0f)
			{
				// See if we hit a player.
				if (IsLunging())
				{
					if (hit.collider.CompareTag(Tags.kPlayerTag))
					{
						ZMPlayerController otherPlayer = hit.collider.GetComponent<ZMPlayerController>();

						if (_playerInPath && otherPlayer._playerInPath)
						{
							if (_movementDirection != otherPlayer._movementDirection)
							{
								Utilities.StopDelayRoutine(_endLungeCoroutine);
								_moveModState = MoveModState.RECOIL;
							}
						}
						else if (otherPlayer._moveModState == MoveModState.PARRY_FACING
								 && IsOpposingDirection(otherPlayer))
						{
							if (otherPlayer._canStun)
							{
								_moveModState = MoveModState.STUN;
								_audio.PlayOneShot(_audioRecoil);
							}
							else
							{
								_moveModState = MoveModState.RECOIL;
								_audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
							}
						}
						else if (otherPlayer._moveModState == MoveModState.PARRY_AOE)
						{
							_moveModState = MoveModState.RECOIL;
						}
						else
						{
							_playerInPath = false;
							KillOpponent (otherPlayer);
						}
					}
					else if (hit.collider.CompareTag("Breakable"))
					{
						hit.collider.GetComponent<ZMBreakable>().HandleCollision(_playerInfo);
					}
				}
			}

			// Check for collision with volume.
			if (hit.collider.CompareTag(Tags.kWarpVolume))
			{
				GetComponent<ZMWarpController>().OnTriggerEnterCC2D(hit.collider);
			}
		}
	}
	
	void onTriggerEnterEvent(Collider2D collider)
	{
		if (collider.CompareTag(Tags.kGrass))
		{
			var grassController = collider.GetComponent<ZMGrassController>();

			// Signal that the player has entered the grass.
			grassController.GrassEnter();

			if (IsLunging() || IsPlunging())
			{
				// Cut the grass if the player is attacking.
				grassController.CutGrass(_playerInfo);
			}
		}
	}
	
	void onTriggerExitEvent(Collider2D collider)
	{
		if (collider.CompareTag(Tags.kGrass))
		{
			collider.GetComponent<ZMGrassController>().GrassExit();
		}
	}

	void HandleMinScoreReached(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			gameObject.SetActive(false);

			_moveModState = MoveModState.ELIMINATED;

			Notifier.SendEventNotification(OnPlayerEliminated, this);
		}
	}

	public void Respawn(Vector3 position)
	{
		Reset();
		
		transform.position = position;
		
		Notifier.SendEventNotification(OnPlayerRespawn, this);
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

			if (Environment.CheckRight(transform.position, RIGHT_EDGE_OFFSET, 2.0f, _controller.platformMask))
			{
				shiftedPos.x -= 4.0f;
			}
			else if (Environment.CheckLeft(transform.position, LEFT_EDGE_OFFSET, 2.0f, _controller.platformMask))
			{
				shiftedPos.x += 4.0f;
			}

			transform.position = shiftedPos;
		}
	}

	private void CheckSkidding()
	{
		if (_controller.isGrounded)
		{
			var isFacingRight = _movementDirection == MovementDirectionState.FACING_RIGHT;
			int direction;
			Quaternion rotation;
			Vector2 skidEffectPosition;
			float yRotation;

			if (isFacingRight)
			{
				direction = 1;
				yRotation = 0.0f;
			}
			else
			{
				direction = -1;
				yRotation = 180.0f;
			}

			rotation = Quaternion.Euler (new Vector3 (0.0f, yRotation, 0.0f));
			skidEffectPosition = new Vector2(transform.position.x + 30 * direction, transform.position.y - 20);

			Instantiate(_effectSkidObject, skidEffectPosition, rotation);
		}
	}

	private void SetVelocity(Vector2 velocity)
	{
		runSpeed = velocity.x;
		_velocity.y = velocity.y;
	}

	private void KillSelf(ZMPlayerController killer)
	{
		GameObject body;

		_moveModState = MoveModState.RESPAWN;
		_controlModState = ControlModState.NEUTRAL;
		_controlMoveState = ControlMoveState.NEUTRAL;

		Notifier.SendEventNotification(OnPlayerDeath, _playerInfo);

		Utilities.StopDelayRoutine(_endLungeCoroutine);
		Utilities.StopDelayRoutine(_resetControlModCoroutine);
		Utilities.StopDelayRoutine(_enablePlayerCoroutine);

		// Handle death visuals here
		_material.color = Color.red;
		_light.enabled = false;
		_spriteRenderer.enabled = false;

		// load and instantiate the body's upper half
		body = GameObject.Instantiate(_upperBodyTemplate) as GameObject;
		body.transform.position = transform.position;

		ZMAddForce upperBody = body.GetComponent<ZMAddForce>();
		upperBody.ParticleColor = _light.color;
		upperBody.AddForce(new Vector2(killer.runSpeed / 12, 0));

		// load and instantiate the body's lower half
		body = GameObject.Instantiate(_lowerBodyTemplate) as GameObject;
		body.transform.position = transform.position;

		ZMAddForce lowerBody = body.GetComponent<ZMAddForce>();
		lowerBody.ParticleColor = _light.color;
		lowerBody.AddForce(new Vector2(killer.runSpeed / 12, 0));

		// Set player states
		_playerInPath = false;

		ClearInputEvents();

		_audio.PlayOneShot(_audioGore[Random.Range (0, _audioGore.Length)]);
		_audio.PlayOneShot(_audioHurt[Random.Range (0, _audioHurt.Length)]);
		_audio.PlayOneShot(_audioKill[Random.Range (0, _audioKill.Length)]);
		_goreEmitter.Play();
	}

	private void KillOpponent(ZMPlayerController playerController) 
	{
		if (playerController.IsAbleToDie())
		{
			playerController.KillSelf(this);

			Notifier.SendEventNotification(OnPlayerKill, this);

			// add the stat
			ZMStatTracker.Kills.Add(_playerInfo);
		}
	}

	private void DisableInputWithCallbackDelay(float delay)
	{
		ClearInputEvents();
		Utilities.ExecuteAfterDelay(AcceptInputEvents, delay);
	}

	private void Reset()
	{
		_controlModState = ControlModState.NEUTRAL;
		_controlMoveState = ControlMoveState.NEUTRAL;
		runSpeed = 0.0f;

		_spriteRenderer.enabled = true;
		_light.enabled = true;

		EnablePlayer();
		SetMovementDirection(transform.position.x < 0 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);

		if (!MatchStateManager.IsPause() && !MatchStateManager.IsEnd())
		{
			AcceptInputEvents();
		}
	}

	private void Recoil()
	{
		float recoilSpeed = 700f;

		if (_movementDirection == MovementDirectionState.FACING_LEFT)
		{
			runSpeed = recoilSpeed;
		}
		else if (_movementDirection == MovementDirectionState.FACING_RIGHT)
		{
			runSpeed = -recoilSpeed;
		}

		_velocity.y = JUMP_HEIGHT / 2.0f;
	}

	private void Plunge()
	{
		runSpeed = 0.0f;
		_velocity.y = -PLUNGE_SPEED;
	}

	private void LungeRight()
	{
		runSpeed = LUNGE_SPEED;
	}

	private void LungeLeft()
	{
		runSpeed = -LUNGE_SPEED;
	}

	private void EndLunge()
	{
		float lagTime = (_moveModState == MoveModState.LUNGING_AIR ? PARRY_TIME_AIR : PARRY_TIME_LUNGE);

		runSpeed = 0;
		_playerInPath = false;

		_moveModState = MoveModState.PARRY_FACING;
		_controlModState = ControlModState.NEUTRAL;
		_enablePlayerCoroutine = Utilities.ExecuteAfterDelay(EnablePlayer, lagTime);

		// Set a cooldown before we can lunge again.
		_canLunge = false;
		DisablePlayer();
		Utilities.ExecuteAfterDelay(LungeCooldown, lagTime);
	}

	private void EndParry()
	{
		_moveModState = MoveModState.NEUTRAL;
		_controlModState = ControlModState.NEUTRAL;
		_canLunge = true;
		_light.color = _baseColor;
	}

	private void EndStunBeginParry()
	{
		_canStun = false;
		Utilities.ExecuteAfterDelay(EndParry, PARRY_TIME);
	}

	private void EndStun()
	{
		_canStun = false;
		EndParry();
	}

	private bool IsAbleToReceiveInput()
	{
		return enabled && _moveModState != MoveModState.RESPAWN && gameObject.activeInHierarchy;
	}

	private bool IsPlunging()
	{
		return _moveModState == MoveModState.PLUNGE || _moveModState == MoveModState.PLUNGING;
	}

	private bool IsLunging()
	{
		return _moveModState == MoveModState.LUNGE || _moveModState == MoveModState.LUNGING_GROUND || _moveModState == MoveModState.LUNGING_AIR;
	}

	private bool IsRecoiling()
	{
		return _moveModState == MoveModState.RECOIL || _moveModState == MoveModState.RECOILING;
	}

	private bool IsParrying()
	{
		return _moveModState == MoveModState.PARRY_FACING || _moveModState == MoveModState.PARRY_AOE;
	}

	public bool IsDead()
	{
		return _moveModState == MoveModState.RESPAWN;
	}

	private bool IsAttacking()
	{
		return  _controlModState == ControlModState.ATTACKING || _controlModState == ControlModState.ATTACK;
	}

	private bool IsTouchingEitherSide()
	{
		return _checkTouchingLeft || _checkTouchingRight;
	}

	private bool IsMovingLeft()
	{
		return _controlMoveState == ControlMoveState.MOVING && _movementDirection == MovementDirectionState.FACING_LEFT;
	}

	private bool IsMovingRight()
	{
		return _controlMoveState == ControlMoveState.MOVING && _movementDirection == MovementDirectionState.FACING_RIGHT;
	}

	private bool IsTouchingLeftAndMovingLeft()
	{
		return _checkTouchingLeft &&  runSpeed < 0;
	}

	private bool IsTouchingRightAndMovingRight()
	{
		return _checkTouchingRight && runSpeed > 0;
	}

	private bool IsAbleToDie()
	{
		return _moveModState != MoveModState.RESPAWN && !IsRecoiling();
	}

	private bool IsOpposingDirection(ZMPlayerController otherPlayer)
	{
		return _movementDirection != otherPlayer._movementDirection;
	}
}
