using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;

public class ZMKillCounter : ZMPlayerItem
{
	private int _kills = 0;

	// references
	private Text _text;

	protected override void Awake()
	{
		base.Awake();

		_text = GetComponent<Text>();
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMPlayerController.PlayerKillEvent += HandlePlayerKillEvent;

		UpdateUI();
	}

	private void HandlePlayerKillEvent(ZMPlayerController killer)
	{
		if (_playerInfo == killer.PlayerInfo)
		{
			_kills += 1;
			UpdateUI();
		}
	}

	private void UpdateUI()
	{
		_text.text = _kills.ToString();
	}
}
