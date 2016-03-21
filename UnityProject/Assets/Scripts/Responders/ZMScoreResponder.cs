using UnityEngine;
using ZMPlayer;

public class ZMScoreResponder : ZMResponder
{
	public bool activeOnScore = true;

	private ZMPlayerInfo _playerInfo;

	protected override void Awake()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMStageScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMStageScoreController.OnStopScore += HandleStopScoreEvent;
	}

	void Start()
	{
		SetActive(!activeOnScore);
	}

	void HandleCanScoreEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			SetActive(activeOnScore);
		}
	}

	void HandleStopScoreEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			SetActive(!activeOnScore);
		}
	}
}
