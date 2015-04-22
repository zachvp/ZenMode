using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using ZMPlayer;

public class ZMPlayerController : MonoBehaviour
{
	// Movement constants.
	private float GRAVITY = 2200.0f;
	private float RUN_SPEED_MAX = 500.0f;
	private float ACCELERATION = 30.0f; // 30.0f
	private float FRICTION = 25.0f; // 25.0f
	private float JUMP_HEIGHT = 800.0f;
	private float PLUNGE_SPEED = 2200.0f;
	private float LUNGE_SPEED = 1800.0f;
	private float LUNGE_TIME = 0.14f;
	private float WALL_SLIDE_SPEED = 80.0f;
	private float WALL_JUMP_KICK_SPEED = 500.0f;
	private float runSpeed = 0.0f;

	// Additional constants.
	private float TILE_SIZE = 28.0f;
	private float RESPAWN_TIME = 2.0f;
	private float LUNGE_COOLDOWN = 0.3f;
	private int FRAMES_PER_STEP = 30;

	private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
	private CharacterController2D _controller;
	private Animator _animator;
	private RaycastHit2D _lastControllerColliderHit;
	private RaycastHit2D _checkTouchingLeft;
	private RaycastHit2D _checkTouchingRight;
	private Vector3 _velocity;
	private int _framesUntilStep = 0;
	private string[] kDeathStrings;
	private bool _canLunge;

	// Speeds of two players before recoil.
	private Vector2 _collisionVelocities;

	// Player states.
	private enum MovementDirectionState { FACING_LEFT, FACING_RIGHT };
	private enum ControlMoveState 		{ NEUTRAL, MOVING };
	private enum ControlModState	    { NEUTRAL, JUMPING, ATTACK, ATTACKING, WALL_JUMPING, PLUNGE, PLUNGING };
	private enum MoveModState 		    { NEUTRAL, PLUNGE, PLUNGING, LUNGE, LUNGING, WALL_SLIDE, RECOIL, RECOILING, DISABLE, DISABLED, RESPAWN };
	private enum AbilityState 			{ NEUTRAL, SHOOTING };

	private ControlMoveState _controlMoveState;
	private ControlModState _controlModState;
	private MovementDirectionState _movementDirection;
	private MoveModState _moveModState;
	private bool _playerInPath;

	// Tags and layers.
	private const string kPlayerTag						    = "Player";
	private const string kWarpVolumeTag						= "WarpVolume";
	private const string kGroundLayerMaskName 			    = "Ground";
	private const string kSpecialInteractiblesLayerMaskName = "SpecialInteractibles";
	
	// Methods.
	private const string kMethodNameEnablePlayer  			= "EnablePlayer";
	private const string kMethodNameEndLunge 	  			= "EndLunge";
	private const string kGameStateControllerName 			= "GameController";
	private const string kRespawnMethodName 	  			= "Respawn";
	
	// Debug.
	private Color _initialColor;
	private Sprite _baseSprite;

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
	public AudioClip _audioRecoil;
	

	// Delegates
	public delegate void PlayerDeathAction(ZMPlayerController playerController); public static event PlayerDeathAction PlayerDeathEvent;
	public delegate void PlayerRespawnAction(ZMPlayerController playerController); public static event PlayerRespawnAction PlayerRespawnEvent;
	public delegate void PlayerRunBroadcast(ZMPlayerController playerController, bool isRunning); public static event PlayerRunBroadcast PlayerRunEvent;
	public delegate void PlayerRecoilAction(); public static event PlayerRecoilAction PlayerRecoilEvent;
	public delegate void PlayerLandPlungeAction(); public static event PlayerLandPlungeAction PlayerLandPlungeEvent;

	void Awake()
	{
		_moveModState = MoveModState.NEUTRAL;
		//_abilityState = AbilityState.NEUTRAL;
		_initialColor = this.renderer.material.color;
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
		_canLunge = true;

		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;
		_controller.onCollisionEnterEvent += onCollisionEnterEvent;

		ZMPlayerInputController.MoveRightEvent += MoveRightEvent;
		ZMPlayerInputController.MoveLeftEvent  += MoveLeftEvent;
		ZMPlayerInputController.NoMoveEvent	   += NoMoveEvent;
		ZMPlayerInputController.JumpEvent	   += JumpEvent;
		ZMPlayerInputController.AttackEvent	   += AttackEvent;
		ZMPlayerInputController.PlungeEvent    += PlungeEvent;

		// Set original facing direction.
		SetMovementDirection(transform.position.x > 0 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);
	}

	void Start() 
	{
		_baseSprite = GetComponent<SpriteRenderer>().sprite;
		kDeathStrings = new string[31];
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
		kDeathStrings[30] = "OOOOHHHH";
	}

	void FixedUpdate()
	{
		// Set raycasts.
		_checkTouchingLeft  = CheckLeft(TILE_SIZE, _controller.platformMask);
		_checkTouchingRight = CheckRight(TILE_SIZE, _controller.platformMask);

		// Update the velocity calculated from the controller.
		_velocity = _controller.velocity;
		
		// Disable / enable player.
		if (ShouldDisableWhenGrounded()) {
			_moveModState = MoveModState.DISABLED;
			DisablePlayer();
			Invoke(kMethodNameEnablePlayer, 0.1f);
		} else if (ShouldEnable()) {
			EnablePlayer();
		}
		
		// Check raycasts.
		if (_controller.isGrounded) {
			// Landing.
			if (_velocity.y < -1) {
				audio.PlayOneShot(_audioLand[Random.Range (0, _audioLand.Length)], 0.5f);
				Instantiate(_effectLandObject, new Vector2(transform.position.x - 3, transform.position.y - 8), transform.rotation);
			}
			_velocity.y = 0;

			if (IsPerformingPlunge()) {
				audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);

				if (PlayerLandPlungeEvent != null) {
					PlayerLandPlungeEvent();
				}
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

			if (_controller.isGrounded) {
				_framesUntilStep++;
				if (_framesUntilStep >= FRAMES_PER_STEP) {
					_framesUntilStep = 0;
					audio.PlayOneShot(_audioStep[Random.Range (0, _audioStep.Length)], 0.1f);
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

			audio.PlayOneShot(_audioJump[Random.Range (0, _audioJump.Length)]);
			Instantiate(_effectJumpObject, new Vector2(transform.position.x - 3, transform.position.y - 8), transform.rotation);
		}

		// Update movement and ability state.
		if (_controlModState == ControlModState.ATTACK) {
			_controlModState = ControlModState.ATTACKING;
			if (!IsPerformingLunge ()) {
				if (_canLunge) {
					audio.PlayOneShot(_audioLunge[Random.Range (0, _audioLunge.Length)]);
					_moveModState = MoveModState.LUNGE;

					Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 180.0f : 0.0f), 0.0f));
					Instantiate (_effectLungeObject, new Vector2 (transform.position.x - 3, transform.position.y - 10), rotation);

					// Set a cooldown before we can lunge again.
					Invoke ("LungeCooldown", LUNGE_COOLDOWN);
					_canLunge = false;
				}
			}
		} else if (_controlModState == ControlModState.PLUNGE) {
			_controlModState = ControlModState.PLUNGING;
			if (!IsPerformingPlunge()) {
				audio.PlayOneShot(_audioPlunge[Random.Range (0, _audioPlunge.Length)]);
				_moveModState = MoveModState.PLUNGE;
			}
		}
		else if (IsTouchingEitherSide()) {
			if (!_controller.isGrounded && _moveModState == MoveModState.NEUTRAL) {
				_moveModState = MoveModState.WALL_SLIDE;
			}
		}

		if (_moveModState == MoveModState.PLUNGE) {
			_moveModState = MoveModState.PLUNGING;
			Plunge();
		} else if (_moveModState == MoveModState.PLUNGING) {
			if (_controller.isGrounded) {
				Instantiate(_effectPlungeObject, new Vector2(transform.position.x, transform.position.y), transform.rotation);
				_moveModState = MoveModState.NEUTRAL;

				Invoke("ResetControlModState", 0.1f);
			}
		} else if (_moveModState == MoveModState.LUNGE) {
			_moveModState = MoveModState.LUNGING;

			RaycastHit2D checkPlayer;
			if (_movementDirection == MovementDirectionState.FACING_RIGHT && !_checkTouchingRight) {
				if (checkPlayer = CheckRight(145f, _controller.specialInteractibleMask)) {
					if (checkPlayer.collider.CompareTag(kPlayerTag) && !_playerInPath) {
						_playerInPath = true;
					}
				}

				LungeRight();
			} else if (_movementDirection == MovementDirectionState.FACING_LEFT && !_checkTouchingLeft) {
				if (checkPlayer = CheckLeft(145f, _controller.specialInteractibleMask)) {
					if (checkPlayer.collider.CompareTag(kPlayerTag) && !_playerInPath) {
						_playerInPath = true;
					}
				}

				LungeLeft();
			}

			if (IsInvoking(kMethodNameEndLunge)) CancelInvoke(kMethodNameEndLunge);
			Invoke (kMethodNameEndLunge, LUNGE_TIME);
		}

		if (_moveModState == MoveModState.RECOIL) {
			audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
			Instantiate(_effectClashObject, new Vector2(transform.position.x, transform.position.y), transform.rotation);
			_moveModState = MoveModState.RECOILING;

			if (PlayerRecoilEvent != null) {
				PlayerRecoilEvent();
			}

			Recoil();
		} else if (_moveModState == MoveModState.RECOILING) {
			_moveModState = MoveModState.NEUTRAL;
			_controlModState = ControlModState.NEUTRAL;

			_playerInPath = false;
		} else if (_moveModState == MoveModState.WALL_SLIDE) {
			// Wall slide.
			if (_velocity.y < 1.0f &&  _controlMoveState == ControlMoveState.MOVING) {
				_velocity.y = -WALL_SLIDE_SPEED;
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
						audio.PlayOneShot(_audioJump[Random.Range (0, _audioJump.Length)]);
						Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 180.0f : 0.0f), 0.0f));
						Instantiate (_effectSkidObject, new Vector2 (transform.position.x, transform.position.y - 20), rotation);
						
						if (IsMovingLeft()) {
							runSpeed = WALL_JUMP_KICK_SPEED * 0.6f;
						}
						else if (IsMovingRight()) {
							runSpeed = -WALL_JUMP_KICK_SPEED * 0.6f;
						}
						
						_moveModState = MoveModState.NEUTRAL;
					}
				}
			}
		}

		// Handle material color.
		if (IsAbleToKill()) {
			this.renderer.material.color = Color.yellow;
		} else if (_moveModState == MoveModState.DISABLED) {
			this.renderer.material.color = Color.grey;
		} else if (_moveModState == MoveModState.RESPAWN) {
			this.renderer.material.color = Color.white;
		} else {
			this.renderer.material.color = _initialColor;
		}

		// Update and apply velocity.
		_velocity.x = runSpeed;
		_velocity.y -= GRAVITY * Time.deltaTime;
		if (IsPerformingLunge ()) {
			_velocity.y = 0.0f;
		}
		_controller.move( _velocity * Time.deltaTime );

		// Update animation states.
		_animator.SetBool ("isRunning", (_controlMoveState == ControlMoveState.MOVING));
		
		bool isSkidding = ((_movementDirection == MovementDirectionState.FACING_LEFT && _velocity.x > 0) ||
		                   (_movementDirection == MovementDirectionState.FACING_RIGHT && _velocity.x < 0));
		bool isSliding = (_velocity.x != 0 && _controlMoveState == ControlMoveState.NEUTRAL);

		_animator.SetBool ("isSkidding", isSkidding || isSliding);
		_animator.SetBool ("isGrounded", _controller.isGrounded);
		_animator.SetBool ("isPlunging", IsPerformingPlunge());
		_animator.SetBool ("isLunging", IsPerformingLunge());
		_animator.SetFloat ("velocityY", _velocity.y);

		if (PlayerRunEvent != null) {
			PlayerRunEvent(this, _controlMoveState == ControlMoveState.MOVING);
		}
	}
	
	void LungeCooldown() {
		_canLunge = true;
		_controlModState = ControlModState.NEUTRAL;
	}

	void OnDestroy() {
		PlayerDeathEvent   	  = null;
		PlayerRespawnEvent 	  = null;
		PlayerRunEvent 	   	  = null;
		PlayerRecoilEvent  	  = null;
		PlayerLandPlungeEvent = null;
	}
	
	// Event handling
	private void MoveRightEvent (ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals(_playerInfo) && enabled) {
			_controlMoveState = ControlMoveState.MOVING;
			if (_movementDirection == MovementDirectionState.FACING_LEFT) {
				CheckSkidding ();
			}
			SetMovementDirection(MovementDirectionState.FACING_RIGHT);
		}
	}

	private void MoveLeftEvent(ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals(_playerInfo) && enabled) {
			_controlMoveState = ControlMoveState.MOVING;
			if (_movementDirection == MovementDirectionState.FACING_RIGHT) {
				CheckSkidding ();
			}
			SetMovementDirection(MovementDirectionState.FACING_LEFT);
		}
	}

	private void NoMoveEvent(ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals(_playerInfo)) {
			_controlMoveState = ControlMoveState.NEUTRAL;
		}
	}

	private void JumpEvent(ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals(_playerInfo)) {
			if (_controller.isGrounded && _controlModState != ControlModState.JUMPING) {
				_controlModState = ControlModState.JUMPING;
			} else if (IsTouchingEitherSide()) {
				_controlModState = ControlModState.WALL_JUMPING;
			}
		}
	}

	private void AttackEvent(ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals(_playerInfo) && !IsAttacking() && _moveModState != MoveModState.RESPAWN) {
			_controlModState = ControlModState.ATTACK;
		}
	}

	private void PlungeEvent(ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals (_playerInfo) && !IsAttacking() && !_controller.isGrounded && _moveModState != MoveModState.RESPAWN) {
			_controlModState = ControlModState.PLUNGE;
		}
	}

	void ResetControlModState() {
		_controlModState = ControlModState.NEUTRAL;
	}

	void onControllerCollider( RaycastHit2D hit )
	{
		if (hit.collider.gameObject.layer == LayerMask.NameToLayer(kSpecialInteractiblesLayerMaskName)) {
			if (hit.normal.y == 1.0f) {
				if (hit.collider.CompareTag(kPlayerTag)) {
					ZMPlayerController otherPlayer = hit.collider.GetComponent<ZMPlayerController>();
					if (IsPerformingPlunge() && otherPlayer.IsAbleToDie()) {
						KillOpponent (hit.collider.gameObject);
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
						} else if (otherPlayer.IsAbleToDie()) {
							_playerInPath = false;
							KillOpponent (hit.collider.gameObject);
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
		}
	}
	
	void onTriggerExitEvent( Collider2D collider )
	{
		ZMWarpController warpController = gameObject.GetComponent<ZMWarpController>();
		if (warpController != null) {
			warpController.OnTriggerEnterCC2D(collider);
		}

		if (collider.CompareTag ("Grass")) {
			collider.GetComponent<ZMGrassController>().GrassExit();
		}
	}
	
	void onCollisionEnterEvent(Collision2D collision) 
	{
	}

	// Player state utility methods
	public void EnablePlayer()
	{
		_moveModState = MoveModState.NEUTRAL;
		//_abilityState  = AbilityState.NEUTRAL;

		_controller.enabled = true;
		this.enabled = true;

		// Play our unsheathing animation, and play a sound.
		_animator.SetBool ("didBecomeActive", true);
		// TODO: Add a sound here.
	}

	public void DisablePlayer() 
	{
		_controller.enabled = false;
		this.enabled = false;
	}

	private void SetMovementDirection(MovementDirectionState direction)
	{
		MovementDirectionState previousDirection = _movementDirection;

		_movementDirection = direction;
		
		// Modify x-scale and flip our sprite based on our direction.
		float scaleFactor = (direction == MovementDirectionState.FACING_LEFT ? -1 : 1);
		transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * scaleFactor,
		                                   transform.localScale.y,
		                                   transform.localScale.z);

		if (!previousDirection.Equals(_movementDirection)) {
			Vector3 shiftedPos = transform.position;

			if (CheckRight(2.0f, _controller.platformMask)) {
				shiftedPos.x -= 8.0f;
			} else if (CheckLeft(2.0f, _controller.platformMask)) {
				shiftedPos.x += 8.0f;
			}

			transform.position = shiftedPos;
		}
	}

	void CheckSkidding() {
		if (_controller.isGrounded) {
			int direction = (_movementDirection == MovementDirectionState.FACING_RIGHT ? 1 : -1);
			Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 0.0f : 180.0f), 0.0f));
			Instantiate (_effectSkidObject, new Vector2 (transform.position.x + 30 * direction, transform.position.y - 20), rotation);
		}
	}

	private void KillSelf()
	{
		_moveModState = MoveModState.RESPAWN;
		_controlModState = ControlModState.NEUTRAL;
		_controlMoveState = ControlMoveState.NEUTRAL;

		// Handle death visuals here
		this.renderer.material.color = Color.red;
		light.enabled = false;

		DisablePlayer();
		Invoke(kRespawnMethodName, RESPAWN_TIME);

		// Set player states
		_playerInPath = false;
		runSpeed = 0f;

		// Notify event handlers of player's death
		if (PlayerDeathEvent != null) {
			PlayerDeathEvent(this);
		}

		audio.PlayOneShot(_audioGore[Random.Range (0, _audioGore.Length)]);
		audio.PlayOneShot(_audioHurt[Random.Range (0, _audioHurt.Length)]);
		audio.PlayOneShot(_audioKill[Random.Range (0, _audioKill.Length)]);
		_goreEmitter.Play();

		// Handle taunt text.
		_tauntText.gameObject.SetActive (true);
		_tauntText.text = kDeathStrings [Random.Range (0, kDeathStrings.Length)];
		_tauntText.transform.rotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, Random.Range (-20, 20)));
		_tauntText.transform.position = new Vector3 (Random.Range (-100, 100), Random.Range (-100, 100), 0.0f);
		StartCoroutine (ScaleTauntText (new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, 0.05f));
		Invoke ("HideTauntText", 0.5f);

	}

	private void KillOpponent(GameObject opponent) 
	{
		ZMPlayerController playerController = opponent.GetComponent<ZMPlayerController>();

		playerController.KillSelf();

		ZMMetricsCollector collector = GetComponent<ZMMetricsCollector>();
		if (collector != null) {
			if (IsPerformingLunge()) {
				collector.AddDeathData(0);
			} else {
				collector.AddDeathData(1);
			}
		}

		// TODO: fire death event
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

	private void Respawn() {
		GetComponent<SpriteRenderer>().sprite = _baseSprite;
		light.enabled = true;
		_velocity.y = 100f;
		
		EnablePlayer();

		if (PlayerRespawnEvent != null) {
			PlayerRespawnEvent(this);
		}
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

		_moveModState = MoveModState.NEUTRAL;
		_controlModState = ControlModState.NEUTRAL;
		_playerInPath = false;
	}

	private RaycastHit2D CheckLeft(float distance, LayerMask mask) {
		Vector2 rayOrigin = new Vector2(transform.position.x - 17f, transform.position.y);
		Vector2 rayDirection = new Vector2(-1.0f, 0.0f);

		return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
	}

	private RaycastHit2D CheckRight(float distance, LayerMask mask) {
		Vector2 rayOrigin = new Vector2(transform.position.x + 17f, transform.position.y);
		Vector2 rayDirection = new Vector2(1.0f, 0.0f);

		return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
	}

	private RaycastHit2D CheckBelowPlatform(float distance) {
		Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y);
		Vector2 rayDirection = new Vector2(0.0f, -1.0f);
		
		return Physics2D.Raycast(rayOrigin, rayDirection, distance, _controller.platformMask);
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
		return _moveModState == MoveModState.LUNGE || _moveModState == MoveModState.LUNGING;
	}

	private bool IsRecoiling() {
		return _moveModState == MoveModState.RECOIL || _moveModState == MoveModState.RECOILING;
	}

	private bool IsAbleToKill() {
		return IsPerformingLunge() || IsPerformingPlunge();
	}

	private bool IsAbleToDie() {
		return _moveModState != MoveModState.RESPAWN && !IsRecoiling();
	}

	private bool ShouldDisableWhenGrounded() {
		return _moveModState == MoveModState.DISABLE && _controller.isGrounded;
	}

	private bool ShouldEnableWhenGrounded() {
		return _moveModState == MoveModState.NEUTRAL && _controller.isGrounded;
	}

	private bool ShouldEnable() {
		return !_controller.isGrounded && !IsPerformingPlunge() && !IsRecoiling() && !IsPerformingLunge();
	}

	private bool ShouldRecoilWithPlayer(ZMPlayerController other) {
		return IsPerformingLunge() && other.IsPerformingLunge();
	}

	private bool IsAttacking() {
		return  _controlModState == ControlModState.ATTACKING || _controlModState == ControlModState.ATTACK;
	}
}
