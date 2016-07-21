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

		ZMLobbyScoreController.OnUpdateScore += HandleUpdateScore;
		ZMLobbyScoreController.OnReachMaxScore += HandleDeactivateScoreBar;
		ZMLobbyController.OnPlayerDropOut += HandleDeactivateScoreBar;

		ZMPlayerController.OnPlayerCreate += HandlePlayerCreate;
	}

	protected override void ConfigureScoreSlider()
	{
		_scoreSlider.maxValue = 100.0f;
	}

	private void HandlePlayerCreate(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			ActivateScoreBar(_playerInfo);
			UpdateScore(args.info, 0.0f);
		}
	}
}
