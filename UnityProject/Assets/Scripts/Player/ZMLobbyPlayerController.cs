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

	private void HandleLobbyMaxScoreReachedEvent(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			DisablePlayer();
			GetComponent<Renderer>().enabled = false;
			gameObject.SetActive(false);
		}
	}

	private void HandleOnPlayerDropOut(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			DisablePlayer();
			ClearInputEvents();
			GetComponent<Renderer>().enabled = false;
			gameObject.SetActive(false);
		}
	}
}
