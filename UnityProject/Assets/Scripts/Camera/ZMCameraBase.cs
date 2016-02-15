using UnityEngine;
using ZMPlayer;

public class ZMCameraBase : MonoBehaviour
{
	[SerializeField] protected float endZoom = 432;
	[SerializeField] protected bool zoomAtStart;
	[SerializeField] protected float zoomDelay = 6;
	[SerializeField] protected float startZoom = 200;

	protected ZMMovementBobbing _movementBobbing;

	protected bool _isShaking;
	protected bool _isZooming;
	protected float _speed;

	private Vector3 _startPosition;

	private int _zoomStep;
	private int _zoomFrames;

	private float _zoomTargetSize;
	private float _baseSpeed;

	protected virtual void Awake()
	{
		_baseSpeed = 3.0f;
		_speed = _baseSpeed;
		_startPosition = transform.position;

		_movementBobbing = GetComponent<ZMMovementBobbing>();
	}

	protected virtual void Start()
	{
		_movementBobbing.enabled = false;
		
		if (zoomAtStart) { Invoke("StartZoom", zoomDelay); }
	}

	protected virtual void Update()
	{
		if (_isZooming)
		{
			GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, _zoomTargetSize, _speed * Time.deltaTime);
			
			if (Mathf.Abs(GetComponent<Camera>().orthographicSize - _zoomTargetSize) < 1f)
			{
				_isZooming = false;
				_speed = _baseSpeed;
			}
		}
		
		if (_zoomStep < _zoomFrames) { _zoomStep += 1; }
		else { StopShake(); }
	}

	protected void AcceptPlayerEvents()
	{
		ZMPlayerController.PlayerDeathEvent 	 += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRecoilEvent 	 += HandlePlayerRecoilEvent;
		ZMPlayerController.PlayerLandPlungeEvent += HandlePlayerLandPlungeEvent;
	}

	protected void Shake(int frames)
	{
		_movementBobbing.enabled = true;

		_zoomStep = 0;
		_zoomFrames = frames;
		_isShaking = true;
	}
	
	protected void StopShake()
	{
		_movementBobbing.enabled = false;
		_isShaking = false;
	}
	
	protected void Zoom(float size)
	{
		_zoomTargetSize = size;
		_isZooming = true;
	}
	
	protected void Zoom(float size, Vector3 position)
	{
		MoveTo(position);
		Zoom(size);
	}

	protected void HandleStopShake()
	{
		if (_isShaking) { StopShake(); }
	}

	private void StartZoom()
	{
		Zoom(startZoom);
	}
	
	private void MoveTo(Vector3 position)
	{
		GetComponent<Camera>().transform.position = new Vector3(position.x, position.y, _startPosition.z);
	}
	
	private void HandlePlayerLandPlungeEvent()
	{
		Shake(10);
	}
	
	private void HandlePlayerDeathEvent(ZMPlayerInfo info)
	{
		Shake(25);
	}
	
	private void HandlePlayerRecoilEvent(ZMPlayerController playerController, float stunTime)
	{
		Shake(25);
	}
}
