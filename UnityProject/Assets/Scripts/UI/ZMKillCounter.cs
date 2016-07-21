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

		ZMPlayerController.OnPlayerKill += HandlePlayerKillEvent;
		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;

		UpdateUI(0);
	}

	private void HandleDropOutEvent(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			UpdateUI(0);
		}
	}

	private void HandlePlayerKillEvent(ZMPlayerControllerEventArgs args)
	{
		if (_playerInfo == args.controller.PlayerInfo)
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
