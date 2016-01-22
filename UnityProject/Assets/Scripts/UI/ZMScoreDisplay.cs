using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;

[RequireComponent(typeof(Slider))]
public class ZMScoreDisplay : MonoBehaviour
{
	protected ZMPlayerInfo _playerInfo;
	protected Slider _scoreSlider;
	protected Text _scoreStatus;

	protected float _maxSliderValue;

	protected virtual void Awake()
	{
		_maxSliderValue = ZMScoreController.MAX_SCORE;

		_scoreSlider = GetComponent<Slider>();
		_scoreStatus = GetComponentInChildren<Text>();
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMScoreController.MinScoreReached += EliminateScore;
		ZMScoreController.OnUpdateScore += UpdateScore;

		ConfigureScoreSlider();
		ConfigureScoreStatuses();
	}

	protected void ConfigureScoreSlider()
	{
		_scoreSlider.handleRect = null;
		_scoreSlider.maxValue = _maxSliderValue;
		_scoreSlider.value = 0.0f;
	}
	
	protected void ConfigureScoreStatuses()
	{
		_scoreStatus.text = "";
	}

	protected void ActivateScoreBar(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { gameObject.SetActive(true); }
	}

	protected void DeactivateScoreBar(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { gameObject.SetActive(false); }
	}
	
	protected void EliminateScore(ZMScoreController controller)
	{
		if (_playerInfo == controller.PlayerInfo) { _scoreStatus.text = "ELIMINATED!"; }
	}

	protected void UpdateScore(ZMPlayerInfo info, float amount)
	{		
		if (_playerInfo == info) { _scoreSlider.value = amount; }
	}
}
