using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;

[RequireComponent(typeof(Slider))]
public abstract class ZMScoreDisplay : MonoBehaviour
{
	protected ZMPlayerInfo _playerInfo;
	protected Slider _scoreSlider;
	
	protected virtual void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();

		GetDisplayElements();
		ConfigureScoreSlider();

		ZMScoreController.OnUpdateScore += HandleUpdateScore;
	}

	protected virtual void GetDisplayElements()
	{
		_scoreSlider = GetComponent<Slider>();
	}

	protected void ActivateScoreBar(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			gameObject.SetActive(true);
		}
	}

	protected void HandleDeactivateScoreBar(ZMPlayerInfoEventArgs args)
	{
		DeactivateScoreBar(args.info);
	}

	protected void DeactivateScoreBar(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			gameObject.SetActive(false);
		}
	}
	
	protected void HandleUpdateScore(ZMPlayerInfoFloatEventArgs args)
	{		
		UpdateScore(args.info, args.value);
	}

	protected void UpdateScore(ZMPlayerInfo info, float amount)
	{
		if (_playerInfo == info && _scoreSlider)
		{
			_scoreSlider.value = amount;
		}
	}

	protected abstract void ConfigureScoreSlider();
}
