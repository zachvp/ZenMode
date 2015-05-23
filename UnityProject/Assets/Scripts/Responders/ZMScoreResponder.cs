using UnityEngine;
using ZMPlayer;

public class ZMScoreResponder : MonoBehaviour {
	public bool activeOnScore = true;

	private ZMPlayerInfo _playerInfo;

	// Use this for initialization
	void Awake () {
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMScoreController.StopScoreEvent += HandleStopScoreEvent;

		gameObject.SetActive(!activeOnScore);
	}

	void HandleCanScoreEvent(ZMScoreController scoreController) {
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			gameObject.SetActive(activeOnScore);
		}
	}

	void HandleStopScoreEvent (ZMScoreController scoreController) {
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			gameObject.SetActive(!activeOnScore);
		}
	}
}
