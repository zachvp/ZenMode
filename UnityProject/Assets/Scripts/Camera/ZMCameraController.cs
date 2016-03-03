using UnityEngine;
using Core;
using ZMConfiguration;

[RequireComponent(typeof(Camera))]
public class ZMCameraController : ZMCameraBase
{
	public static EventHandler<Camera> OnCameraStart;

	private Camera _camera;
	private float _totalDistance;

	protected override void Awake()
	{
		base.Awake();

		_camera = GetComponent<Camera>();

		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;

		MatchStateManager.OnMatchStart += HandleStartGameEvent;
		MatchStateManager.OnMatchEnd   += HandleStopShake;
		MatchStateManager.OnMatchPause += HandleStopShake;

		AcceptPlayerEvents();
	}

	protected override void Start()
	{
		base.Start();

		Notifier.SendEventNotification(OnCameraStart, _camera);
	}

	protected void OnDestroy()
	{
		OnCameraStart = null;
	}
	
	private void HandleStartGameEvent()
	{
		Zoom(endZoom);
		_speed = 1.0f;
	}
	
	private void HandleAtPathEndEvent(ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag(Tags.kMainCamera))
		{
			Zoom(endZoom);
			_speed = 1.0f;
		}
	}
}
