using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMPlayer;

public class ZMPedestalController : MonoBehaviour {
	public enum MoveType { CYCLE, RANDOM };
	public MoveType moveType;
	public ParticleSystem zenPop;

	public float moveSpeed;
	public float lingerAfterSpawnTime = 3.0f;

	private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	private enum ScoreState { SCORING_ENABLED, SCORING_DISABLED };
	private enum MoveState  { NEUTRAL, MOVE, MOVING, AT_TARGET };
	private ScoreState _scoreState;
	private MoveState _moveState;
	private Light _pedestalLight;

	// movement
	private List<Transform> _waypoints;
	private int _waypointIndex;
	private Vector3 _targetPosition;
	private float _distanceFromStart;
	private float _totalDistance;

	// references
	private HashSet<ZMScoreController> _scoringAgents;

	private const string kPedestalWaypointTag = "PedestalWaypoint";
	private const string kDisableMethodName   = "Disable";

	// delegates
	public delegate void ActivateAction(ZMPedestalController pedestalController); public static ActivateAction ActivateEvent;

	public delegate void DeactivateAction(ZMPedestalController pedestalController); public static DeactivateAction DeactivateEvent;

	void Awake() {
		_waypoints = new List<Transform>();
		_scoringAgents = new HashSet<ZMScoreController>();
		_playerInfo = GetComponent<ZMPlayerInfo>();

		_moveState = MoveState.MOVE;

		// event handler subscriptions
		ZMPlayerController.PlayerDeathEvent    += HandlePlayerDeathEvent;
		ZMScoreController.UpdateScoreEvent     += HandleUpdateScoreEvent;
		ZMScoreController.CanScoreEvent 	   += HandleCanScoreEvent;
		ZMScoreController.StopScoreEvent   	   += HandleStopScoreEvent;
		ZMScoreController.MinScoreReached	   += HandleMinScoreReached;
		ZMGameStateController.SpawnObjectEvent += HandleSpawnObjectEvent;
	}

	void Start () {
		_pedestalLight = gameObject.GetComponent<Light>();

		foreach (GameObject waypointObject in GameObject.FindGameObjectsWithTag(kPedestalWaypointTag)) {
			_waypoints.Add(waypointObject.transform);
		}

		// Type-specific actions
		if (moveType == MoveType.RANDOM)
			Disable();	
	}

	void FixedUpdate() {
		if (_moveState == MoveState.MOVE) {
			_distanceFromStart = 0.0f;
			_targetPosition = _waypoints[_waypointIndex].position;
			_totalDistance = (_targetPosition - gameObject.transform.position).magnitude;
			
			//_moveState = MoveState.MOVING;
			
			// update waypoint index
			if(moveType == MoveType.CYCLE) {
				_waypointIndex += 1;
				_waypointIndex %= _waypoints.Count;
				
			} else if (moveType == MoveType.RANDOM) {
				_waypointIndex = Random.Range(0, _waypoints.Count);
			}
			
		}

		if (_moveState == MoveState.MOVING) {
			_distanceFromStart += moveSpeed * Time.deltaTime;
			float distanceRatio = _distanceFromStart / _totalDistance;

			gameObject.transform.position = Vector3.Lerp(gameObject.transform.position,
			                                             _targetPosition,
			                                             distanceRatio);

			float clampDistance = 4.0f;
			if ((_targetPosition - gameObject.transform.position).sqrMagnitude < clampDistance * clampDistance) {
				_moveState = MoveState.AT_TARGET;
			}
		} else if (_moveState == MoveState.AT_TARGET) {
			_moveState = MoveState.MOVE;
		}
	}

	void OnDestroy() {
		// unsubscribe all event listeners
		ActivateEvent  = null;
		DeactivateEvent = null;
	}

	void OnTriggerEnter2D(Collider2D collider) {
		/*if (collider.CompareTag("Player")) {
			ZMPlayerController playerController = collider.GetComponent<ZMPlayerController>();

			if (!playerController.IsDead()) {
				if (_scoreState != ScoreState.SCORING_DISABLED) {
					zenPop.renderer.material.color = renderer.material.color;
					zenPop = ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem;
					zenPop = ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem;
					zenPop = ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem;

					Disable();
				}
			}
		}*/
	}

	private void ToggleOn() {
		Invoke("ToggleEnabled", 2.0f);
	}

	private void ToggleOff() {
		Invoke("ToggleEnabled", 2.0f);
	}

	// private methods
	private void ToggleEnabled() {
		if (_scoreState == ScoreState.SCORING_ENABLED) {
			Disable();
			ToggleOn();
		} else if (_scoreState == ScoreState.SCORING_DISABLED) {
			Enable();
			ToggleOff();
		}
	}

	// public methods
	public void Enable() {
		_scoreState = ScoreState.SCORING_ENABLED;
		renderer.enabled = true;

		if (_pedestalLight != null)
			_pedestalLight.enabled = true;

		// notify event handlers
		if (ActivateEvent != null) {
			ActivateEvent(this);
		}
	}

	private void Disable() {
		_scoreState = ScoreState.SCORING_DISABLED;
		renderer.enabled = false;

		if (_pedestalLight != null) {
			_pedestalLight.enabled = false;
		}

		if (DeactivateEvent != null) {
			DeactivateEvent(this);
		}
	}

	private void MoveToLocation(Vector3 location) {
		Vector3 newLocation = new Vector3(location.x, location.y, transform.position.z);

		_moveState = MoveState.MOVE;
		gameObject.transform.position = newLocation;
	}

	public bool IsEnabled() { return _scoreState == ScoreState.SCORING_ENABLED; }
	public bool IsDiabled() { return _scoreState == ScoreState.SCORING_DISABLED; }

	// event handlers
	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			MoveToLocation(playerController.transform.position);
			Enable();

			if (IsInvoking(kDisableMethodName)) {
				CancelInvoke(kDisableMethodName);
			}
		}
	}

	void HandleMinScoreReached (ZMScoreController scoreController)
	{
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			ZMScoreController.MinScoreReached -= HandleMinScoreReached;

			zenPop.renderer.material.color = renderer.material.color;
			zenPop = ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem;
			zenPop = ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem;
			zenPop = ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem;

			Destroy(gameObject);
		}
	}

	void HandleUpdateScoreEvent(ZMScoreController scoreController) {
		float scoreSum = 0;

		foreach (ZMScoreController agent in _scoringAgents) {
			scoreSum += agent.TotalScore;
		}
	}

	void HandleCanScoreEvent(ZMScoreController scoreController) {
		if (!_scoringAgents.Contains(scoreController)) {
			_scoringAgents.Add(scoreController);
		}
	}

	void HandleStopScoreEvent (ZMScoreController scoreController) {
		_scoringAgents.Remove(scoreController);
	}

	void HandleSpawnObjectEvent(ZMGameStateController gameStateController, ZMPlayerController playerController) {
		if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			_moveState = MoveState.MOVE;
			Invoke(kDisableMethodName, lingerAfterSpawnTime);
		}
	}
}
