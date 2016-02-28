using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

public class ZMStageScoreDisplay : ZMScoreDisplay
{
	private Text _scoreStatus;

	protected override void Awake ()
	{
		base.Awake ();

		ZMScoreController.OnReachMinScore += EliminateScore;

		ConfigureScoreStatus();
	}

	protected override void GetDisplayElements()
	{
		base.GetDisplayElements();

		_scoreStatus = GetComponentInChildren<Text>();
	}

	protected void ConfigureScoreStatus()
	{
		_scoreStatus.text = "";
	}

	protected override void ConfigureScoreSlider()
	{
		_scoreSlider.maxValue = ZMScoreController.MAX_SCORE;
	}

	private void EliminateScore(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { _scoreStatus.text = "ELIMINATED!"; }
	}
}
