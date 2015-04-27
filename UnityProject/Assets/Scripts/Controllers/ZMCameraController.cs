using UnityEngine;
using System.Collections;

public class ZMCameraController : MonoBehaviour {
	public float endZoom = 432;

	private float _zoomTargetSize;
	private bool  _isZooming;
	private float _baseSpeed = 3f;
	private float _speed;

	private float _totalDistance;
	private int _zoomStep;
	private int _zoomFrames;

	private Vector3 _basePosition;

	private bool _isShaking = false;

	private ZMMovementBobbing _movementBobbing;

	// Use this for initialization
	void Awake () {
		_speed = _baseSpeed;

		ZMPlayerController.PlayerRecoilEvent 	 += HandlePlayerRecoilEvent;
		ZMPlayerController.PlayerLandPlungeEvent += HandlePlayerLandPlungeEvent;
		ZMPlayerController.PlayerDeathEvent 	 += HandlePlayerDeathEvent;

		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;

		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
		ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
		ZMGameStateController.GameEndEvent   += HandleGameEndEvent;

		_movementBobbing = GetComponent<ZMMovementBobbing>();
	}

	void Start() {
		if (_movementBobbing != null)
			_movementBobbing.enabled = false;
	}

	void HandleGameEndEvent ()
	{
		if (_isShaking) {
			StopShake();
		}
	}

	void HandlePauseGameEvent ()
	{
		if (_isShaking) {
			StopShake();
		}
	}

	void HandleStartGameEvent ()
	{
		Zoom(432);
		_speed = 1.0f;
	}

	void HandleAtPathEndEvent (ZMWaypointMovement lobbyPedestalController)
	{
		Zoom(endZoom);
		_speed = 1.0f;
	}

	void HandlePlayerLandPlungeEvent ()
	{
		Shake(10);
	}

	void HandlePlayerDeathEvent (ZMPlayerController controller)
	{
		Shake(25);
		//_basePosition = transform.position;
		//_speed = 10f;
		//Zoom(200, controller.transform.position);
		//Time.timeScale = 0.15f;

		//Invoke("ResetZoom", 0.5f);
	}

	void Update() {
		if (_isZooming) {
			camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, _zoomTargetSize, _speed * Time.deltaTime);

			if (Mathf.Abs(camera.orthographicSize - _zoomTargetSize) < 1f) {
				_isZooming = false;
				_speed = _baseSpeed;
			}
		}

		if (_zoomStep < _zoomFrames) {
			_zoomStep += 1;
		} else {
			StopShake();
		}
	}

	void HandlePlayerRecoilEvent ()
	{
		Shake(25);
	}
	
	void Shake(int frames) {
		//SendMessage("BobbingOn");
		if (_movementBobbing != null)
			_movementBobbing.enabled = true;
		_zoomStep = 0;
		_zoomFrames = frames;
		_isShaking = true;
		//Invoke("StopShake", time);
	}

	void StopShake() {
		SendMessage("BobbingOff");
		if (_movementBobbing != null)
			_movementBobbing.enabled = false;
		_isShaking = false;
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
		camera.transform.position = new Vector3(position.x, position.y, -192.0f);
	}

	void ResetZoom() {
		Zoom (endZoom, _basePosition);
		_speed = 10f;
		//Time.timeScale = 1.0f;
	}
}
