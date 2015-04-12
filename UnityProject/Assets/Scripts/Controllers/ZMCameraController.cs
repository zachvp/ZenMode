using UnityEngine;
using System.Collections;

public class ZMCameraController : MonoBehaviour {
	public float endZoom = 432;

	private float _zoomTargetSize;
	private bool  _isZooming;
	private float _speed = 3f;

	private float _totalDistance;
	private int _zoomStep;
	private int _zoomFrames;

	// Use this for initialization
	void Awake () {
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
		ZMPlayerController.PlayerRecoilEvent += HandlePlayerRecoilEvent;
		ZMPlayerController.PlayerLandPlungeEvent += HandlePlayerLandPlungeEvent;
		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		ZMLobbyPedestalController.AtPathEndEvent += HandleAtPathEndEvent;
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void HandleStartGameEvent ()
	{
		Zoom(432);
		_speed = 1.0f;
	}

	void HandleAtPathEndEvent (ZMLobbyPedestalController lobbyPedestalController)
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
	}
	
	void Start() {
		GetComponent<ZMMovementBobbing>().enabled = false;
	}

	void FixedUpdate() {
		if (_isZooming) {
			camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, _zoomTargetSize, _speed * Time.deltaTime);

			if (Mathf.Abs(camera.orthographicSize - _zoomTargetSize) < 1f) {
				_isZooming = false;
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

	/*void Shake(float time) {
		GetComponent<ZMMovementBobbing>().enabled = true;
		//Invoke("StopShake", time);
	}*/

	void Shake(int frames) {
		GetComponent<ZMMovementBobbing>().enabled = true;
		_zoomStep = 0;
		_zoomFrames = frames;
		//Invoke("StopShake", time);
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
