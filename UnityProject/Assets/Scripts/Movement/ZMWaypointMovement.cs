using UnityEngine;
using Core;

public class ZMWaypointMovement : MonoBehaviour
{
	public float moveSpeed = 8f;
	public bool moveAtStart = false;
	public float startMoveDelay = 3.0f;

	public static EventHandler<ZMWaypointMovement, int> AtPathNodeEvent;
	public static EventHandler<ZMWaypointMovement> AtPathEndEvent;

	protected Transform[] _waypoints;
	protected int _waypointSize; // in case not all waypoints will be reached right away
	
	// movement
	private enum MoveState { STOPPED, MOVE, MOVING, AT_TARGET, COMPLETED };
	
	private MoveState _moveState;
	private int _waypointIndex;
	private float _distanceToTarget;
	private float _distanceTraveled;
	private Vector3 _targetPosition;

	void Awake()
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
				_moveState = MoveState.MOVE;
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
	
	// Update is called once per frame
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
			}
		}
		else if (_moveState == MoveState.AT_TARGET)
		{
			if (_waypointIndex < _waypointSize)
			{
				_moveState = MoveState.MOVE;
				
				Notifier.SendEventNotification(AtPathNodeEvent, this, _waypointIndex);
			}
			else if (_waypointIndex == _waypointSize)
			{
				_moveState = MoveState.COMPLETED;
				_waypointIndex += 1;

				Notifier.SendEventNotification(AtPathEndEvent, this);
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

	private void Move(int index)
	{
		_waypointIndex = index;
	}

	private void Move()
	{
		_waypointIndex = 0;
		_moveState = MoveState.MOVE;
		enabled = true;
	}
}
