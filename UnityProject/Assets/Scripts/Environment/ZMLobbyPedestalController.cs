using UnityEngine;
using System.Collections;

public class ZMLobbyPedestalController : MonoBehaviour {
	public Transform[] waypoints;
	public float moveSpeed = 1.5f;
	public bool endPathAtStart = false;
	public bool deactivateAtStart = false;

	public delegate void AtPathNodeAction(ZMLobbyPedestalController lobbyPedestalController); public static event AtPathNodeAction AtPathNodeEvent;
	public delegate void AtPathEndAction(ZMLobbyPedestalController lobbyPedestalController); public static event AtPathEndAction AtPathEndEvent;
	public delegate void FullPathCycleAction(ZMLobbyPedestalController lobbyPedestalController); public static event FullPathCycleAction FullPathCycleEvent;

	// movement
	private enum MoveState { STOPPED, MOVE, MOVING, AT_TARGET };

	private MoveState _moveState;
	private Vector3 _basePosition;
	private int _waypointIndex;
	private float _distanceToTarget;
	private float _distanceTraveled;
	private Vector3 _targetPosition;

	// ID
	ZMPlayer.ZMPlayerInfo _playerInfo;

	// Use this for initialization
	void Awake() {
		_moveState = MoveState.STOPPED;
		if (deactivateAtStart)
			gameObject.SetActive(false);

		if (light != null)
			light.enabled = false;

		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (playerTag.Equals(_playerInfo.playerTag)) {
			gameObject.SetActive(true);
			light.enabled = true;
		}
	}

	void HandleStartGameEvent ()
	{
		ReturnToBase();
		_waypointIndex = waypoints.Length;
	}

	void Start () {
		_basePosition = transform.position;
		Move ();
		//Invoke("Move", 0.5f);
	}

	void OnDestroy() {
		AtPathNodeEvent    = null;
		AtPathEndEvent 	   = null;
		FullPathCycleEvent = null;

		ZMGameStateController.StartGameEvent -= HandleStartGameEvent;
		ZMLobbyScoreController.MaxScoreReachedEvent -= HandleMaxScoreReachedEvent;
	}
	
	void FixedUpdate() {
		if (_moveState == MoveState.MOVE && _waypointIndex < waypoints.GetLength(0)) {
			_distanceTraveled = 0.0f;
			_targetPosition = waypoints[_waypointIndex].position;
			_distanceToTarget = (_targetPosition - transform.position).magnitude;
			
			_moveState = MoveState.MOVING;
			
			// update waypoint index
			_waypointIndex += 1;
			//_waypointIndex %= waypoints.Count;			
		}

		if (_moveState == MoveState.MOVING) {
			_distanceTraveled += moveSpeed * Time.deltaTime;
			float distanceRatio = _distanceTraveled / _distanceToTarget;
			
			gameObject.transform.position = Vector3.Lerp(transform.position, _targetPosition, distanceRatio);

			if ((_targetPosition - gameObject.transform.position).sqrMagnitude < 4.0f * 4.0f) {
				_moveState = MoveState.AT_TARGET;
			}
		} else if (_moveState == MoveState.AT_TARGET) {
			if (_waypointIndex < waypoints.GetLength(0)) {
				_moveState = MoveState.MOVE;

				if (AtPathNodeEvent != null) {
					AtPathNodeEvent(this);
				}
			} else if (_waypointIndex == waypoints.GetLength(0)) {
				if (AtPathEndEvent != null) {
					AtPathEndEvent(this);
				}

				if (endPathAtStart) {
					_waypointIndex += 1;
					
					ReturnToBase();
				} else {
					_moveState = MoveState.STOPPED;
				}
			} else {
				// has returned back to start
				_moveState = MoveState.STOPPED;

				if (FullPathCycleEvent != null) {
					FullPathCycleEvent(this);
				}
			}
		}
	}

	void ReturnToBase() {
		_distanceTraveled = 0.0f;
		_targetPosition = _basePosition;
		_distanceToTarget = (_targetPosition - transform.position).magnitude;
		
		_moveState = MoveState.MOVING;
	}
	
	void Move() {
		_moveState = MoveState.MOVE;
		_waypointIndex = 0;
	}

	void HandleMaxScoreReachedEvent (ZMLobbyScoreController lobbyScoreController)
	{
		if (lobbyScoreController.GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag.Equals(GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag)) {
			Destroy(gameObject);
		}
	}
}
