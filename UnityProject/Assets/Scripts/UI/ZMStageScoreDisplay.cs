using UnityEngine;
using UnityEngine.UI;

public class ZMStageScoreDisplay : ZMScoreDisplay
{
	private Text _scoreStatus;

	protected override void Awake ()
	{
		base.Awake ();

		ZMScoreController.MinScoreReached += EliminateScore;

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

	private void EliminateScore(ZMScoreController controller)
	{
		if (_playerInfo == controller.PlayerInfo) { _scoreStatus.text = "ELIMINATED!"; }
	}
}
