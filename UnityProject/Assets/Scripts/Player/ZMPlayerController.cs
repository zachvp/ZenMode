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
	private float ACCELERATION = 30.0f;
	private float FRICTION = 25.0f;
	private float JUMP_HEIGHT = 800.0f;
	private float PLUNGE_SPEED = 3000.0f;
	private float LUNGE_SPEED = 2400.0f;
	private float LUNGE_TIME = 0.1f;
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
	private bool _canWallJump;
	private bool _canAirParry;

	// Speeds of two players before recoil.
	private Vector2 _collisionVelocities;

	// Player states.
	private enum MovementDirectionState { FACING_LEFT, FACING_RIGHT };
	private enum ControlMoveState 		{ NEUTRAL, MOVING };
	private enum ControlModState	    { NEUTRAL, JUMPING, ATTACK, ATTACKING, WALL_JUMPING, PLUNGE, PLUNGING, PARRY, PARRYING };
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
	private const string kRespawnMethodName 	  			= "Respawn";

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
	public AudioClip[] _audioBash;

	// DISMEMBERMENT!
	public GameObject _bodyUpperHalf;

	// Delegates
	public delegate void PlayerKillAction(ZMPlayerController killer); public static event PlayerKillAction PlayerKillEvent;
	public delegate void PlayerDeathAction(ZMPlayerController playerController); public static event PlayerDeathAction PlayerDeathEvent;
	public delegate void PlayerRespawnAction(ZMPlayerController playerController); public static event PlayerRespawnAction PlayerRespawnEvent;
	public delegate void PlayerEliminatedAction(ZMPlayerController playerController); public static event PlayerEliminatedAction PlayerEliminatedEvent;
	public delegate void PlayerRecoilAction(ZMPlayerController playerController, float stunTime); public static event PlayerRecoilAction PlayerRecoilEvent;
	public delegate void PlayerStunAction(ZMPlayerController playerController, float stunTime); public static event PlayerStunAction PlayerStunEvent;
	public delegate void PlayerParryAction(ZMPlayerController playerController, float parryTime); public static event PlayerParryAction PlayerParryEvent;
	public delegate void PlayerLandPlungeAction(); public static event PlayerLandPlungeAction PlayerLandPlungeEvent;

	// Debug
	SpriteRenderer _spriteRenderer;
	Color _baseColor;

	void Awake()
	{
		_moveModState = MoveModState.NEUTRAL;
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_animator = GetComponent<Animator>();
		_controller = GetComponent<CharacterController2D>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
		_canLunge = true;
		_canWallJump = true;

		_controller.onControllerCollidedEvent += onControllerCollider;
		_controller.onTriggerEnterEvent += onTriggerEnterEvent;
		_controller.onTriggerExitEvent += onTriggerExitEvent;

		ZMPlayerInputController.MoveRightEvent += MoveRightEvent;
		ZMPlayerInputController.MoveLeftEvent  += MoveLeftEvent;
		ZMPlayerInputController.NoMoveEvent	   += NoMoveEvent;
		ZMPlayerInputController.JumpEvent	   += JumpEvent;
		ZMPlayerInputController.AttackEvent	   += AttackEvent;
		ZMPlayerInputController.PlungeEvent    += PlungeEvent;
		ZMPlayerInputController.ParryEvent     += ParryEvent;
		ZMScoreController.MinScoreReached += HandleMinScoreReached;

		// Set original facing direction.
		SetMovementDirection(transform.position.x > 0 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);
	}

	void Start() 
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

		_baseColor = light.color;
		_goreEmitter.renderer.material.color = _baseColor;
		_goreEmitter.startColor = _baseColor;
	}

	void FixedUpdate()
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

			Debug.Log(gameObject.name + ": " + _moveModState);
			_canAirParry = true;
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

			/*
			if (Mathf.Abs(runSpeed) < Mathf.Abs (RUN_SPEED_MAX)) {
				runSpeed = (_movementDirection == MovementDirectionState.FACING_LEFT ? Mathf.Abs(runSpeed) * -1 : Mathf.Abs(runSpeed));
				runSpeed += ACCELERATION * (_movementDirection == MovementDirectionState.FACING_LEFT ? -1 : 1);
			}
			*/

			if (_controller.isGrounded && Mathf.Abs(_velocity.x) > ACCELERATION) {
				_framesUntilStep++;
				if (_framesUntilStep >= FRAMES_PER_STEP) {
					_framesUntilStep = 0;
					audio.PlayOneShot(_audioStep[Random.Range (0, _audioStep.Length)], 0.25f);
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
				}
			}
		} else if (_controlModState == ControlModState.PLUNGE) {
			_controlModState = ControlModState.PLUNGING;
			if (!IsPerformingPlunge()) {
				audio.PlayOneShot(_audioPlunge[Random.Range (0, _audioPlunge.Length)]);
				_moveModState = MoveModState.PLUNGE;
			}
		} else if (_controlModState == ControlModState.PARRY) {
			_spriteRenderer.color = Color.yellow;
			light.color = Color.white;

			_controlModState = ControlModState.PARRYING;
			_moveModState = MoveModState.PARRY_FACING;
			_controlMoveState = ControlMoveState.NEUTRAL;
			_canStun = true;
			_canLunge = false;
			runSpeed = 0;

			if (_controller.isGrounded) {
				Invoke("EndStunBeginParry", PARRY_STUN_WINDOW);

				if (PlayerParryEvent != null) {
					PlayerParryEvent(this, PARRY_STUN_WINDOW + PARRY_TIME);
				}
			} else if (_canAirParry){
				Debug.Log(gameObject.name + "Air Parry");
				Invoke("EndStun", PARRY_STUN_WINDOW);
				_canAirParry = false;
			}
		} else if (IsTouchingEitherSide()) {
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

				// AOE Check:
				ZMPlayerController playerController;

				// Check right:
				RaycastHit2D recoilAOE = CheckRight(new Vector2(EDGE_OFFSET, -16.0f), AOE_RANGE, _controller.specialInteractibleMask);
				if (recoilAOE) {
					playerController = recoilAOE.collider.GetComponent<ZMPlayerController>();
					if (playerController != null) {
						playerController._movementDirection = MovementDirectionState.FACING_LEFT;
						playerController._moveModState = MoveModState.RECOIL;
					}
				}
				// Check left:
				recoilAOE = CheckLeft(new Vector2(-EDGE_OFFSET, -16.0f), AOE_RANGE, _controller.specialInteractibleMask);
				if (recoilAOE) {
					playerController = recoilAOE.collider.GetComponent<ZMPlayerController>();
					if (playerController != null) {
						playerController._movementDirection = MovementDirectionState.FACING_RIGHT;
						playerController._moveModState = MoveModState.RECOIL;
					}
				}

				_moveModState = MoveModState.PARRY_AOE;
				DisablePlayer();

				Invoke(kMethodNameEnablePlayer, PARRY_TIME_LUNGE);
				Invoke("ResetControlModState", PARRY_TIME_LUNGE + 0.02f);
			}
		} else if (_moveModState == MoveModState.LUNGE) {
			_moveModState = _controller.isGrounded ? MoveModState.LUNGING_GROUND : MoveModState.LUNGING_AIR;

			RaycastHit2D checkPlayer;
			if (_movementDirection == MovementDirectionState.FACING_RIGHT) {
				if (checkPlayer = CheckRight(145f, _controller.specialInteractibleMask)) {
					if (checkPlayer.collider.CompareTag(kPlayerTag) && !_playerInPath) {
						_playerInPath = true;
					}
				}

				LungeRight();
			} else if (_movementDirection == MovementDirectionState.FACING_LEFT) {
				if (checkPlayer = CheckLeft(145f, _controller.specialInteractibleMask)) {
					if (checkPlayer.collider.CompareTag(kPlayerTag) && !_playerInPath) {
						_playerInPath = true;
					}
				}

				LungeLeft();
			}

			// End the lunge after a delay
			if (IsInvoking(kMethodNameEndLunge)) CancelInvoke(kMethodNameEndLunge);
			Invoke (kMethodNameEndLunge, LUNGE_TIME);
		}

		if (_moveModState == MoveModState.RECOIL) {
			audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);
			Instantiate(_effectClashObject, new Vector2(transform.position.x, transform.position.y), transform.rotation);
			_moveModState = MoveModState.RECOILING;

			if (IsInvoking(kMethodNameEndLunge)) CancelInvoke(kMethodNameEndLunge);

			Recoil();

			if (PlayerRecoilEvent != null) {
				PlayerRecoilEvent(this, RECOIL_STUN_TIME);
			}
		} else if (_moveModState == MoveModState.RECOILING) {
			_moveModState = MoveModState.NEUTRAL;
			_controlModState = ControlModState.NEUTRAL;
			_controlMoveState = ControlMoveState.NEUTRAL;

			_playerInPath = false;
		} else if (_moveModState == MoveModState.STUN) {
			_moveModState = MoveModState.STUNNED;
			_controlModState = ControlModState.NEUTRAL;
			_controlMoveState = ControlMoveState.NEUTRAL;

			audio.PlayOneShot(_audioSword[Random.Range (0, _audioSword.Length)], 1.0f);

			if (IsInvoking(kMethodNameEndLunge)) CancelInvoke(kMethodNameEndLunge);

			Recoil();

			Invoke("ResetMoveModState", STUN_TIME);

			if (PlayerStunEvent != null) {
				PlayerStunEvent(this, STUN_TIME);
			}
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
						audio.PlayOneShot(_audioJump[Random.Range (0, _audioJump.Length)]);
						Quaternion rotation = Quaternion.Euler (new Vector3 (0.0f, (_movementDirection == MovementDirectionState.FACING_RIGHT ? 180.0f : 0.0f), 0.0f));
						float offset = (_movementDirection == MovementDirectionState.FACING_RIGHT ? 12.0f : -12.0f);
						Instantiate (_effectSkidObject, new Vector2 (transform.position.x + offset, transform.position.y - 20), rotation);
						_moveModState = MoveModState.NEUTRAL;
						_canWallJump = false;
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

	void WallJumpCooldown() {
		_canWallJump = true;
	}

	void LungeCooldown() {
		_canLunge = true;
		_controlModState = ControlModState.NEUTRAL;
	}

	void OnDestroy() {
		ZMScoreController.MinScoreReached -= HandleMinScoreReached;

		PlayerKillEvent		  = null;
		PlayerDeathEvent   	  = null;
		PlayerRespawnEvent 	  = null;
		PlayerEliminatedEvent = null;
		PlayerRecoilEvent  	  = null;
		PlayerStunEvent		  = null;
		PlayerParryEvent	  = null;
		PlayerLandPlungeEvent = null;
	}
	
	// Event handling - CCONTROL
	private void MoveRightEvent (ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals(_playerInfo) && enabled) {
			_controlMoveState = ControlMoveState.MOVING;
			if (_movementDirection == MovementDirectionState.FACING_LEFT) {
				CheckSkidding ();
			}

			if (!IsPerformingLunge()) {
				SetMovementDirection(MovementDirectionState.FACING_RIGHT);
			}
		}
	}

	private void MoveLeftEvent(ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals(_playerInfo) && enabled) {
			_controlMoveState = ControlMoveState.MOVING;
			if (_movementDirection == MovementDirectionState.FACING_RIGHT) {
				CheckSkidding ();
			}

			if (!IsPerformingLunge()) {
				SetMovementDirection(MovementDirectionState.FACING_LEFT);
			}
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
			} else if (IsTouchingEitherSide() && _canWallJump) {
				_controlModState = ControlModState.WALL_JUMPING;
			}
		}
	}

	private void AttackEvent(ZMPlayerInputController inputController, int direction) {
		if (inputController.PlayerInfo.Equals(_playerInfo) && !IsAttacking() && _moveModState != MoveModState.RESPAWN) {
			RaycastHit2D hit;
			Vector2 forward = new Vector2(direction, 0);

			_controlModState = ControlModState.ATTACK;

			if (direction != 0) {
				SetMovementDirection(direction == -1 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);
			}

			// hack for destroying a breakable when pressed up against it
			if (_movementDirection == MovementDirectionState.FACING_LEFT) {
				hit = CheckLeft(2.0f, _controller.specialInteractibleMask);
			} else {
				hit = CheckRight(2.0f, _controller.specialInteractibleMask);
			}

			if (hit && Mathf.Round(Vector3.Dot(hit.normal, forward)) != 0 && hit.collider != null) {
				if (hit.collider.CompareTag("Breakable")) {
					hit.collider.GetComponent<ZMBreakable>().HandleCollision(_playerInfo);
				}
			}
		}
	}

	private void PlungeEvent(ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals (_playerInfo) && !IsAttacking() && _moveModState != MoveModState.RESPAWN) {
			if (!_controller.isGrounded) {
				_controlModState = ControlModState.PLUNGE;
			} 
		}
	}

	private void ParryEvent (ZMPlayerInputController inputController) {
		if (inputController.PlayerInfo.Equals (_playerInfo) && !IsParrying () && _moveModState != MoveModState.RESPAWN && _canAirParry) {
			_controlModState = ControlModState.PARRY;
		}
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
					audio.PlayOneShot(_audioBash[Random.Range(0, _audioBash.Length)], runSpeed / RUN_SPEED_MAX);
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
							} else {
								_moveModState = MoveModState.RECOIL;

								audio.PlayOneShot(_audioRecoil);
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
		ZMWarpController warpController = gameObject.GetComponent<ZMWarpController>();
		if (warpController != null) {
			warpController.OnTriggerEnterCC2D(collider);
		}

		if (collider.CompareTag ("Grass")) {
			collider.GetComponent<ZMGrassController>().GrassExit();
		}
	}

	void HandleMinScoreReached (ZMScoreController scoreController)
	{
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			gameObject.SetActive(false);

			_moveModState = MoveModState.ELIMINATED;

			if (PlayerEliminatedEvent != null) {
				PlayerEliminatedEvent(this);
			}
		}
	}

	// Player state utility methods
	public void EnablePlayer()
	{
		if (_moveModState == MoveModState.ELIMINATED) return;

		_moveModState = MoveModState.NEUTRAL;

		_controller.enabled = true;
		this.enabled = true;

		_animator.SetBool ("didBecomeActive", true);
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
				shiftedPos.x -= 4.0f;
			} else if (CheckLeft(2.0f, _controller.platformMask)) {
				shiftedPos.x += 4.0f;
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

	private void AddVelocity(Vector2 velocity) {
		runSpeed = velocity.x;
		_velocity.y = 700; //velocity.y;
	}

	private void KillSelf()
	{
		_moveModState = MoveModState.RESPAWN;
		_controlModState = ControlModState.NEUTRAL;
		_controlMoveState = ControlMoveState.NEUTRAL;
		CancelInvoke(kMethodNameEndLunge);
		CancelInvoke("ResetControlModState");
		CancelInvoke(kMethodNameEnablePlayer);

		// Handle death visuals here
		this.renderer.material.color = Color.red;
		light.enabled = false;
		_spriteRenderer.enabled = false;
		_bodyUpperHalf = GameObject.Instantiate(_bodyUpperHalf) as GameObject;
		_bodyUpperHalf.transform.position = transform.position;
		_bodyUpperHalf.GetComponent<ZMAddForce>().ParticleColor = light.color;

		// Set player states
		_playerInPath = false;

		Invoke(kRespawnMethodName, RESPAWN_TIME);

		audio.PlayOneShot(_audioGore[Random.Range (0, _audioGore.Length)]);
		audio.PlayOneShot(_audioHurt[Random.Range (0, _audioHurt.Length)]);
		audio.PlayOneShot(_audioKill[Random.Range (0, _audioKill.Length)]);
		_goreEmitter.Play();

		// Handle taunt text.
		if (_tauntText) {
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
		if (playerController.IsAbleToDie()) {
			playerController.KillSelf();

			if (PlayerKillEvent != null) {
				PlayerKillEvent(this);
			}

			if (PlayerDeathEvent != null) {
				PlayerDeathEvent(playerController);
			}

			// add the stat
			// ZMStatTracker.Instance.Kills.Add(_playerInfo);
		}

		// apply "forces" to each of the players
//		runSpeed *= -0.4f;
		//playerController.AddVelocity(_velocity);
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
		_controlModState = ControlModState.NEUTRAL;
		_controlMoveState = ControlMoveState.NEUTRAL;
		runSpeed = 0;

		_spriteRenderer.enabled = true;
//		_bodyUpperHalf.transform.position = new Vector3(3, 0, 0);
//		_bodyUpperHalf.SetActive(false);
		light.enabled = true;
		
		EnablePlayer();
		SetMovementDirection(transform.position.x < 0 ? MovementDirectionState.FACING_LEFT : MovementDirectionState.FACING_RIGHT);

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
		_playerInPath = false;

		float lagTime = (_moveModState == MoveModState.LUNGING_AIR ? PARRY_TIME_AIR : PARRY_TIME_LUNGE);
		_moveModState = MoveModState.PARRY_FACING;
		Invoke(kMethodNameEnablePlayer, lagTime);

		// Set a cooldown before we can lunge again.
		Invoke ("LungeCooldown", lagTime);
		_canLunge = false;
		DisablePlayer();
	}

	private void EndParry() {
		_moveModState = MoveModState.NEUTRAL;
		_controlModState = ControlModState.NEUTRAL;

		_spriteRenderer.color = Color.black;
		light.color = _baseColor;

		_canLunge = true;
//		EnablePlayer();
	}

	private void EndStunBeginParry() {
		_canStun = false;
		_spriteRenderer.color = Color.magenta;

		Invoke(kEndParryMethodName, PARRY_TIME);
	}

	private void EndStun() {
		_canStun = false;

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

	private RaycastHit2D CheckBelow(Vector2 offset, float distance, LayerMask mask) {
		Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 32.0f);
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
		return !_controller.isGrounded && !IsPerformingPlunge() && !IsRecoiling() && !IsPerformingLunge()
			   && _moveModState != MoveModState.PARRY_FACING && _moveModState != MoveModState.PARRY_AOE;;
	}

	private bool ShouldRecoilWithPlayer(ZMPlayerController other) {
		return IsPerformingLunge() && other.IsPerformingLunge();
	}

	private bool IsAttacking() {
		return  _controlModState == ControlModState.ATTACKING || _controlModState == ControlModState.ATTACK;
	}
}
