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
		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;

		UpdateUI(0);
	}

	private void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			UpdateUI(0);
		}
	}

	private void HandlePlayerKillEvent(ZMPlayerController killer)
	{
		if (_playerInfo == killer.PlayerInfo)
		{
			UpdateUI(_kills + 1);
		}
	}

	private void UpdateUI(int kills)
	{
		_kills = kills;
		_text.text = _kills.ToString();
	}
}
