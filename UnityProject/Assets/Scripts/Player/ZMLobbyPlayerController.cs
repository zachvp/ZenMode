using UnityEngine;
using ZMPlayer;

public class ZMLobbyPlayerController : ZMPlayerController
{
	protected override void Awake()
	{
		base.Awake();

		EnablePlayer();
		AcceptInputEvents();

		ZMLobbyScoreController.OnReachMaxScore += HandleLobbyMaxScoreReachedEvent;
		ZMLobbyController.OnPlayerDropOut += HandleOnPlayerDropOut;
	}

	protected override void Start()
	{
		base.Start();

		_animator.SetBool("didBecomeActive", true);
	}

	private void HandleLobbyMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			DisablePlayer();
			GetComponent<Renderer>().enabled = false;
			gameObject.SetActive(false);
		}
	}

	private void HandleOnPlayerDropOut(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			DisablePlayer();
			ClearInputEvents();
			GetComponent<Renderer>().enabled = false;
			gameObject.SetActive(false);
		}
	}
}
