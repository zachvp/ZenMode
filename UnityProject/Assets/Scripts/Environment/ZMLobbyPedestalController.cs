using UnityEngine;
using Core;
using ZMPlayer;
using ZMConfiguration;

public class ZMLobbyPedestalController : ZMPlayerItem
{
	public static EventHandler<ZMPlayerInfoEventArgs> ActivateEvent;

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

	void HandleAtPathEndEvent(ZMWaypointMovementEventArgs args)
	{
		if (gameObject == args.movement.gameObject)
		{
			var infoArgs = new ZMPlayerInfoEventArgs(_playerInfo);

			Notifier.SendEventNotification(ActivateEvent, infoArgs);
		}
	}

	void HandlePlayerJoinedEvent(IntEventArgs args)
	{
		if (_playerInfo.ID == args.value)
		{
			gameObject.SetActive(true);
		}
	}

	void HandleMaxScoreReachedEvent(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info) { gameObject.SetActive(false); }
	}
}
