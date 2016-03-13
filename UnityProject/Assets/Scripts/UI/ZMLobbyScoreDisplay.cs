using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;

public class ZMLobbyScoreDisplay : ZMScoreDisplay
{
	protected override void Awake()
	{
		base.Awake();

		DeactivateScoreBar(_playerInfo);

		ZMLobbyScoreController.OnUpdateScore += UpdateScore;
		ZMLobbyScoreController.OnReachMaxScore += DeactivateScoreBar;
		ZMLobbyController.OnPlayerDropOut += DeactivateScoreBar;

		ZMPlayerController.OnPlayerCreate += HandlePlayerCreate;
	}

	protected override void ConfigureScoreSlider()
	{
		_scoreSlider.maxValue = 100.0f;
	}

	private void HandlePlayerCreate(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			ActivateScoreBar(_playerInfo);
			UpdateScore(info, 0.0f);
		}
	}
}
