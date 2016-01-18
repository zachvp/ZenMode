using UnityEngine;
using ZMConfiguration;
using ZMPlayer;

public class ZMStageScoreDisplayManager : ZMScoreDisplayManager
{
	protected override void Awake()
	{
		base.Awake();

		_initialSliderValue = Settings.MatchPlayerCount.value > 2 ? ZMScoreController.MAX_SCORE / 2f : 
																	ZMScoreController.MAX_SCORE / Settings.MatchPlayerCount.value;
	}
}
