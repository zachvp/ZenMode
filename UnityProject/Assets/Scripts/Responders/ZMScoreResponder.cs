using UnityEngine;
using ZMPlayer;

public class ZMScoreResponder : MonoBehaviour
{
	public bool activeOnScore = true;

	private ZMPlayerInfo _playerInfo;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMStageScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMStageScoreController.OnStopScore += HandleStopScoreEvent;
	}

	void Start()
	{
		gameObject.SetActive(!activeOnScore);
	}

	void HandleCanScoreEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			gameObject.SetActive(activeOnScore);
		}
	}

	void HandleStopScoreEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			gameObject.SetActive(!activeOnScore);
		}
	}
}
