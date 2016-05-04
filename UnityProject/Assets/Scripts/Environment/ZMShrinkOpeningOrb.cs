using UnityEngine;
using ZMPlayer;
using ZMConfiguration;

public class ZMShrinkOpeningOrb : ZMScalePulse
{
	[SerializeField] private float shrinkRate = 0.04f;

	private ZMPlayerInfo _playerInfo;

	protected override void Awake()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMWaypointMovement.AtPathNodeEvent += HandleAtPathNodeEvent;
	}

	private void HandleAtPathNodeEvent(ZMWaypointMovement waypointMovement, int index)
	{
		if (index - 1 == _playerInfo.ID)
		{
			StopPulsing();
			ScaleToTargetOverTime(Vector3.zero, shrinkRate);
			_scaleCoroutineCallback.OnFinished += OnFinishedShrink;
		}
	}

	private void OnFinishedShrink()
	{
		gameObject.SetActive(false);
	}
}
