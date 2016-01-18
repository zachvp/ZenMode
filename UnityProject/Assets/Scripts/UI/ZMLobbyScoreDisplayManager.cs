using UnityEngine;

public class ZMLobbyScoreDisplayManager : ZMScoreDisplayManager
{
	protected override void Awake()
	{
		base.Awake();

		_maxSliderValue = 100.0f;

		ZMLobbyScoreController.OnUpdateScore += UpdateScore;
		ZMLobbyScoreController.OnMaxScoreReached += DeactivateScoreBar;
		ZMLobbyController.DropOutEvent += DeactivateScoreBar;

		ZMPlayerController.OnPlayerCreate += HandlePlayerCreate;
	}

	protected override void Start()
	{
		base.Start();

		DeactivateScoreBars();
	}

	private void HandlePlayerCreate(ZMPlayerController controller)
	{
		ActivateScoreBar(controller.PlayerInfo);
		UpdateScore(controller.PlayerInfo, 0.0f);
	}
}
