using UnityEngine;
using Core;

public class ZMCameraController : MonoBehaviour
{
	public float endZoom = 432;
	public bool zoomAtStart;
	public float zoomDelay = 6;
	private float _zoomTargetSize;
	private bool  _isZooming;
	private float _baseSpeed = 3f;
	private float _speed;
	private float _totalDistance;
	private int _zoomStep;
	private int _zoomFrames;
	private bool _isShaking = false;
	private ZMMovementBobbing _movementBobbing;

	void Awake()
	{
		_speed = _baseSpeed;

		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;

		ZMGameStateController.Instance.StartGameEvent += HandleStartGameEvent;

		MatchStateManager.OnMatchEnd   += HandleGameEndEvent;
		MatchStateManager.OnMatchPause += HandlePauseGameEvent;

		_movementBobbing = GetComponent<ZMMovementBobbing>();

		ZMPlayerManager.Instance.OnAllPlayersSpawned += AcceptPlayerEvents;
	}

	void Start()
	{
		if (_movementBobbing != null) { _movementBobbing.enabled = false; }

		if (zoomAtStart) { Invoke("StartZoom", zoomDelay); }
	}

	void Update()
	{
		if (_isZooming)
		{
			camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, _zoomTargetSize, _speed * Time.deltaTime);

			if (Mathf.Abs(camera.orthographicSize - _zoomTargetSize) < 1f)
			{
				_isZooming = false;
				_speed = _baseSpeed;
			}
		}

		if (_zoomStep < _zoomFrames) { _zoomStep += 1; }
		else { StopShake(); }
	}

	private void AcceptPlayerEvents()
	{
		var players = ZMPlayerManager.Instance.Players;
		
		for (int i = 0; i < players.Length; ++i)
		{
			players[i].PlayerDeathEvent 	 += HandlePlayerDeathEvent;
			players[i].PlayerRecoilEvent 	 += HandlePlayerRecoilEvent;
			players[i].PlayerLandPlungeEvent += HandlePlayerLandPlungeEvent;
		}
	}
	
	private void HandleGameEndEvent ()
	{
		if (_isShaking) {
			StopShake();
		}
	}
	
	private void HandlePauseGameEvent()
	{
		if (_isShaking) {
			StopShake();
		}
	}
	
	private void HandleStartGameEvent()
	{
		Zoom(432);
		_speed = 1.0f;
	}
	
	private void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.name.Equals("Main Camera")) {
			Zoom(endZoom);
			_speed = 1.0f;
		}
	}
	
	private void HandlePlayerLandPlungeEvent()
	{
		Shake(10);
	}
	
	private void HandlePlayerDeathEvent(ZMPlayerController controller)
	{
		Shake(25);
	}

	private void HandlePlayerRecoilEvent (ZMPlayerController playerController, float stunTime)
	{
		Shake(25);
	}
	
	private void Shake(int frames)
	{
		if (_movementBobbing != null) { _movementBobbing.enabled = true; }
		_zoomStep = 0;
		_zoomFrames = frames;
		_isShaking = true;
	}

	private void StopShake()
	{
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

	private void StartZoom()
	{
		Zoom (200);
	}
}
