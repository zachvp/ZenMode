using UnityEngine;
using ZMConfiguration;

public class ZMLobbyPlayerManager : ZMPlayerManager
{
	private bool[] _readiedPlayers;
	private static int _playerCount; public static int PlayerCount { get { return _playerCount; } }
	
	protected override void Awake()
	{
		base.Awake ();

		if (_readiedPlayers == null) { _readiedPlayers = new bool[Constants.MAX_PLAYERS]; }

		if (debug)
		{
			_playerCount = debugPlayerCount;
		}
		else
		{
			_playerCount = 0;
		}

		ZMLobbyController.PlayerReadyEvent += HandlePlayerReadyEvent;
		ZMLobbyController.DropOutEvent += HandleDropOutEvent;

		for (int i = 0; i < _players.Length; ++i)
		{
			_players[i].PlayerKillEvent += HandlePlayerKillEvent;
		}
	}

	private void HandleDropOutEvent(int playerIndex)
	{
		_playerCount -= 1;
	}

	private void HandlePlayerReadyEvent (ZMPlayer.ZMPlayerInfo playerTag)
	{
		_playerCount += 1;
	}

	private void HandlePlayerKillEvent(ZMPlayerController killer)
	{		
		Settings.LobbyKillcount.value[killer.PlayerInfo.ID] += 1;
	}
}
