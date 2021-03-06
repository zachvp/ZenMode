﻿using UnityEngine;
using Core;

public class ZMWaypointMovement : MonoBehaviour
{
	public float moveSpeed = 8f;
	public bool moveAtStart = false;
	public float startMoveDelay = 3.0f;

	public static EventHandler<ZMWaypointMovementIntEventArgs> AtPathNodeEvent;
	public static EventHandler<ZMWaypointMovementEventArgs> AtPathEndEvent;

	protected Transform[] _waypoints;
	protected int _waypointSize; // in case not all waypoints will be reached right away
	
	// movement
	private enum MoveState { STOPPED, MOVE, MOVING, AT_TARGET, COMPLETED };
	
	private MoveState _moveState;
	private int _waypointIndex;
	private float _distanceToTarget;
	private float _distanceTraveled;
	private Vector3 _targetPosition;

	protected virtual void Awake()
	{
		enabled = false;

		if (moveAtStart)
		{
			if (startMoveDelay > 0)
			{
				Invoke("Move", startMoveDelay);
			}
			else
			{
				Move(0);
			}
		}
		else
		{
			Stop();
		}
	}

	void Start()
	{
		InitData();
	}

	void OnDestroy()
	{
		AtPathNodeEvent    = null;
		AtPathEndEvent 	   = null;
	}
	
	void FixedUpdate()
	{
		// state sets
		if (_moveState == MoveState.MOVE && _waypointIndex < _waypointSize)
		{
			// set up the movement variables
			_distanceTraveled = 0.0f;

			if (_waypoints[_waypointIndex] != null)
			{
				_targetPosition = _waypoints[_waypointIndex].position;
				_distanceToTarget = (_targetPosition - transform.position).magnitude;
				
				_moveState = MoveState.MOVING;
			}

			_waypointIndex += 1;
		}

		// state checks
		if (_moveState == MoveState.MOVING) {
			float distanceRatio;
			Vector3 newPosition;

			_distanceTraveled += moveSpeed * Time.deltaTime;
			distanceRatio = _distanceTraveled / _distanceToTarget;

			newPosition = Vector3.Lerp(transform.position, _targetPosition, distanceRatio);

			transform.position = newPosition;

			if ((_targetPosition - transform.position).sqrMagnitude < 4.0f * 4.0f)
			{
				_moveState = MoveState.AT_TARGET;
				transform.position = _targetPosition;
			}
		}
		else if (_moveState == MoveState.AT_TARGET)
		{
			if (_waypointIndex < _waypointSize)
			{
				var args = new ZMWaypointMovementIntEventArgs(this, _waypointIndex);

				_moveState = MoveState.MOVE;
				
				Notifier.SendEventNotification(AtPathNodeEvent, args);
			}
			else if (_waypointIndex == _waypointSize)
			{
				var args = new ZMWaypointMovementEventArgs(this);

				_moveState = MoveState.COMPLETED;
				_waypointIndex += 1;

				Notifier.SendEventNotification(AtPathEndEvent, args);
			}
		}
	}

	protected virtual void InitData() { }

	// TODO: Move this to Utilities class.
	protected Transform[] GetWaypoints(string tag)
	{
		var waypoints = GameObject.FindGameObjectsWithTag(tag);
		var transforms = new Transform[waypoints.Length];

		for (int i = 0; i < waypoints.Length; ++i)
		{
			transforms[i] = waypoints[i].transform;
		}

		return transforms;
	}

	private void Stop()
	{
		_moveState = MoveState.STOPPED;
	}

	protected void Move(int index)
	{
		_waypointIndex = index;
		Move();
	}

	private void Move()
	{
		_moveState = MoveState.MOVE;
		enabled = true;
	}
}
