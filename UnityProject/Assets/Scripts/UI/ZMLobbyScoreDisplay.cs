using UnityEngine;

public class ZMLobbyScoreDisplay : ZMScoreDisplay
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

	protected void Start()
	{
		DeactivateScoreBar(_playerInfo);
	}

	private void HandlePlayerCreate(ZMPlayerController controller)
	{
		ActivateScoreBar(_playerInfo);
		UpdateScore(controller.PlayerInfo, 0.0f);
	}
}
