using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

public class ZMKillCounter : MonoBehaviour {
	int _kills = 0;

	// references
	Text _text;
	ZMPlayerInfo _playerInfo;

	void Awake () {
		_text = GetComponent<Text>();
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMPlayerController.PlayerKillEvent += HandlePlayerKillEvent;

		UpdateUI();
	}

	void HandlePlayerKillEvent (ZMPlayerController killer)
	{
		if (_playerInfo == killer.PlayerInfo)
		{
			_kills += 1;
			UpdateUI();
		}
	}

	private void UpdateUI() {
		_text.text = _kills.ToString();
	}
}
