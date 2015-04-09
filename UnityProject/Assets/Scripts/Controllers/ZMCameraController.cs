﻿using UnityEngine;
using System.Collections;

public class ZMCameraController : MonoBehaviour {
	private float   _zoomTargetSize;
	private Vector3 _zoomTargetPosition;
	private bool    _isZooming;
	private bool    _isMoving;
	private float 	_speed = 3f;
	private int 	_moveIndex = 0;
	private float _totalDistance;

	// Use this for initialization
	void Awake () {
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
		ZMPlayerController.PlayerRecoilEvent += HandlePlayerRecoilEvent;
		ZMPlayerController.PlayerLandPlungeEvent += HandlePlayerLandPlungeEvent;
		ZMLobbyPedestalController.AtPathEndEvent += HandleAtPathEndEvent;
	}

	void HandleAtPathEndEvent (ZMLobbyPedestalController lobbyPedestalController)
	{
		Zoom(432);
		_speed = 1.0f;
	}

	void HandlePlayerLandPlungeEvent ()
	{
		Shake(.1f);
	}
	
	void Start() {
		GetComponent<ZMMovementBobbing>().enabled = false;

		camera.orthographicSize = 200;
		_isMoving = true;
		//Zoom(432);
	}

	void FixedUpdate() {
		if (_isZooming) {
			camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, _zoomTargetSize, _speed * Time.deltaTime);

			if (Mathf.Abs(camera.orthographicSize - _zoomTargetSize) < 1f) {
				_isZooming = false;
			}
		}
	}

	void HandleStartGameEvent ()
	{

	}

	void HandlePlayerRecoilEvent ()
	{
		Shake(0.175f);
	}

	void Shake(float time) {
		GetComponent<ZMMovementBobbing>().enabled = true;
		Invoke("StopShake", time);
	}

	void StopShake() {
		GetComponent<ZMMovementBobbing>().enabled = false;
	}

	private void Zoom(float size) {
		_zoomTargetSize = size;
		_isZooming = true;
	}

	private void Zoom(float size, Vector3 position) {
		MoveTo(position);
		Zoom(size);
	}

	private void MoveTo(Vector3 position) {
		camera.transform.position = new Vector3(position.x, position.y, position.z);
	}
}