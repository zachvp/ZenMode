using UnityEngine;
using Core;
using ZMPlayer;
using ZMConfiguration;

public class ZMLobbyPedestalController : ZMPlayerItem
{
	public static EventHandler<ZMPlayerInfo> ActivateEvent;

	protected override void Awake()
	{
		base.Awake();

		ZMScoreController.OnReachMaxScore += HandleMaxScoreReachedEvent;
		ZMLobbyController.OnPlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;
	}

	void OnDestroy()
	{
		ActivateEvent = null;
	}

	void HandleAtPathEndEvent(ZMWaypointMovement waypointMovement)
	{
		if (gameObject == waypointMovement.gameObject)
		{
			Notifier.SendEventNotification(ActivateEvent, _playerInfo);
		}
	}

	void HandlePlayerJoinedEvent(int controlIndex)
	{
		if (_playerInfo.ID == controlIndex)
		{
			gameObject.SetActive(true);
		}
	}

	void HandleMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { gameObject.SetActive(false); }
	}
}
