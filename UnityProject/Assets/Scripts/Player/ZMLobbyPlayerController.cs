using UnityEngine;
using ZMPlayer;

public class ZMLobbyPlayerController : ZMPlayerController
{
	protected override void Awake()
	{
		base.Awake();

		EnablePlayer();
		AcceptInputEvents();

		ZMLobbyScoreController.OnMaxScoreReached += HandleLobbyMaxScoreReachedEvent;
	}

	private void HandleLobbyMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			DisablePlayer();
			renderer.enabled = false;
			gameObject.SetActive(false);
		}
	}
}
