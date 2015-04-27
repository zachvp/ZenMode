using UnityEngine;
using System.Collections;

public class ZMWaypointMovement : MonoBehaviour {
	public Transform[] waypoints;
	public float moveSpeed = 1.5f;
	public bool endPathAtStart = false;
	public bool moveAtStart = false;
	
	public delegate void AtPathNodeAction(ZMWaypointMovement waypointMovement); public static event AtPathNodeAction AtPathNodeEvent;
	public delegate void AtPathEndAction(ZMWaypointMovement waypointMovement); public static event AtPathEndAction AtPathEndEvent;
	public delegate void FullPathCycleAction(ZMWaypointMovement waypointMovement); public static event FullPathCycleAction FullPathCycleEvent;
	
	// movement
	private enum MoveState { STOPPED, MOVE, MOVING, AT_TARGET };
	
	private MoveState _moveState;
	private Vector3 _basePosition;
	private int _waypointIndex;
	private float _distanceToTarget;
	private float _distanceTraveled;
	private Vector3 _targetPosition;

	// Use this for initialization
	void Awake () {
		if (moveAtStart) {
			Move(0);
		} else {
			Stop();
		}

		_basePosition = transform.position;

		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void OnDestroy() {
		AtPathNodeEvent    = null;
		AtPathEndEvent 	   = null;
		FullPathCycleEvent = null;
		
		ZMGameStateController.StartGameEvent -= HandleStartGameEvent;
	}
	
	// Update is called once per frame
	void Update () {
		if (_moveState == MoveState.MOVE && _waypointIndex < waypoints.GetLength(0)) {
			_distanceTraveled = 0.0f;
			_targetPosition = waypoints[_waypointIndex].position;
			_distanceToTarget = (_targetPosition - transform.position).magnitude;
			
			_moveState = MoveState.MOVING;
			
			// update waypoint index
			_waypointIndex += 1;
		}
		
		if (_moveState == MoveState.MOVING) {
			float distanceRatio;

			_distanceTraveled += moveSpeed * Time.deltaTime;
			distanceRatio = _distanceTraveled / _distanceToTarget;
			
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

	void Stop() {
		_moveState = MoveState.STOPPED;
	}

	void Move(int index) {
		_moveState = MoveState.MOVE;
		_waypointIndex = index;
	}

	void ReturnToBase() {
		_distanceTraveled = 0.0f;
		_targetPosition = _basePosition;
		_distanceToTarget = (_targetPosition - transform.position).magnitude;
		
		_moveState = MoveState.MOVING;
	}

	void HandleStartGameEvent ()
	{
		ReturnToBase();
		_waypointIndex = waypoints.Length;
	}
}
