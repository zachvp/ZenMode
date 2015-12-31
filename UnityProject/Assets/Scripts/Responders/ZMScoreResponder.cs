using UnityEngine;
using ZMPlayer;

public class ZMScoreResponder : MonoBehaviour
{
	public bool activeOnScore = true;

	private ZMPlayerInfo _playerInfo;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMScoreController.StopScoreEvent += HandleStopScoreEvent;
	}

	void Start()
	{
		gameObject.SetActive(!activeOnScore);
	}

	void HandleCanScoreEvent(ZMScoreController scoreController)
	{
		if (_playerInfo == scoreController.PlayerInfo)
		{
			gameObject.SetActive(activeOnScore);
		}
	}

	void HandleStopScoreEvent (ZMScoreController scoreController)
	{
		if (_playerInfo == scoreController.PlayerInfo)
		{
			gameObject.SetActive(!activeOnScore);
		}
	}
}
