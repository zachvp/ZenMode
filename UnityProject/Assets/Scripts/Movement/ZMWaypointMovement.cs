﻿using UnityEngine;
using System.Collections;

public class ZMWaypointMovement : MonoBehaviour {
	public Transform[] waypoints;
	public float moveSpeed = 8f;
	public bool moveAtStart = false;
	
	public delegate void AtPathNodeAction(ZMWaypointMovement waypointMovement); public static event AtPathNodeAction AtPathNodeEvent;
	public delegate void AtPathEndAction(ZMWaypointMovement waypointMovement); public static event AtPathEndAction AtPathEndEvent;
	
	// movement
	private enum MoveState { STOPPED, MOVE, MOVING, AT_TARGET, COMPLETED };
	
	private MoveState _moveState;
	private int _waypointIndex;
	private int _waypointSize; // in case not all waypoints will be reached right away
	private float _distanceToTarget;
	private float _distanceTraveled;
	private Vector3 _targetPosition;

	// Use this for initialization
	void Awake () {
		_waypointSize = waypoints.GetLength(0);

		if (moveAtStart) {
			Move(0);
			_moveState = MoveState.MOVE;
		} else {
			Stop();
		}

		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void OnDestroy() {
		AtPathNodeEvent    = null;
		AtPathEndEvent 	   = null;
		
		ZMGameStateController.StartGameEvent -= HandleStartGameEvent;
	}
	
	// Update is called once per frame
	void Update () {
		// state sets
		if (_moveState == MoveState.MOVE && _waypointIndex < _waypointSize) {
			// set up the movement variables
			_distanceTraveled = 0.0f;

			if (waypoints[_waypointIndex] != null) {
				_targetPosition = waypoints[_waypointIndex].position;
				_distanceToTarget = (_targetPosition - transform.position).magnitude;
				
				_moveState = MoveState.MOVING;
			}

			_waypointIndex += 1;
		}

		// state checks
		if (_moveState == MoveState.MOVING) {
			float distanceRatio;

			_distanceTraveled += moveSpeed * Time.deltaTime;
			distanceRatio = _distanceTraveled / _distanceToTarget;
			
			gameObject.transform.position = Vector3.Lerp(transform.position, _targetPosition, distanceRatio);

			if ((_targetPosition - gameObject.transform.position).sqrMagnitude < 4.0f * 4.0f) {
				_moveState = MoveState.AT_TARGET;
			}
		} else if (_moveState == MoveState.AT_TARGET) {
			if (_waypointIndex < _waypointSize) {
				_moveState = MoveState.MOVE;
				
				if (AtPathNodeEvent != null) {
					AtPathNodeEvent(this);
				}
			} else if (_waypointIndex == _waypointSize) {
				_moveState = MoveState.COMPLETED;
				_waypointIndex += 1;

				if (AtPathEndEvent != null) {
					AtPathEndEvent(this);
				}
			}
		}
	}

	private void Stop() {
		_moveState = MoveState.STOPPED;
	}

	private void Move(int index) {
		_waypointIndex = index;
	}

	// event handlers	
	private void HandleStartGameEvent ()
	{
		// cut to the last waypoint
		if (_waypointIndex <= _waypointSize)
			Move(waypoints.GetLength(0) - 1);
	}
}
