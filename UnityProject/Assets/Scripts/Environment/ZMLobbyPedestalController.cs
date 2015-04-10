﻿using UnityEngine;
using System.Collections;

public class ZMLobbyPedestalController : MonoBehaviour {
	public Transform[] waypoints;
	public float moveSpeed = 1.5f;

	public delegate void AtPathEndAction(ZMLobbyPedestalController lobbyPedestalController);
	public static event AtPathEndAction AtPathEndEvent;

	// movement
	private enum MoveState { STOPPED, MOVE, MOVING, AT_TARGET };

	private MoveState _moveState;
	private Vector3 _basePosition;
	private int _waypointIndex;
	private float _distanceToTarget;
	private float _distanceTraveled;
	private Vector3 _targetPosition;

	// Use this for initialization
	void Awake() {
		_moveState = MoveState.STOPPED;

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void HandleStartGameEvent ()
	{
		ReturnToBase();
		_waypointIndex = waypoints.Length;
	}

	void Start () {
		Invoke("Move", 0.5f);
		_basePosition = transform.position;
	}

	void OnDestroy() {
		AtPathEndEvent = null;
		ZMGameStateController.StartGameEvent -= HandleStartGameEvent;

		ZMLobbyScoreController.MaxScoreReachedEvent -= HandleMaxScoreReachedEvent;
	}
	
	void Update() {
		if (_moveState == MoveState.MOVE && _waypointIndex < waypoints.GetLength(0)) {
			_distanceTraveled = 0.0f;
			_targetPosition = waypoints[_waypointIndex].position;
			_distanceToTarget = (_targetPosition - transform.position).magnitude;
			
			_moveState = MoveState.MOVING;
			
			// update waypoint index
			_waypointIndex += 1;
			//_waypointIndex %= waypoints.Count;			
		}
	}
	
	void FixedUpdate() {
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
			} else {
				if (AtPathEndEvent != null) {
					AtPathEndEvent(this);
				}

				ReturnToBase();
				//_moveState = MoveState.STOPPED;
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
