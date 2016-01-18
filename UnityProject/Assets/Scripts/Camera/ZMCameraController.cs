using UnityEngine;
using Core;
using ZMConfiguration;

public class ZMCameraController : ZMCameraBase
{
	private float _totalDistance;

	protected override void Awake()
	{
		base.Awake();

		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;

		MatchStateManager.OnMatchStart += HandleStartGameEvent;
		MatchStateManager.OnMatchEnd   += HandleStopShake;
		MatchStateManager.OnMatchPause += HandleStopShake;

		AcceptPlayerEvents();
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
