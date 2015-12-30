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
		ZMPlayerController.PlayerKillEvent += HandlePlayerKillEvent;
		ZMLobbyController.DropOutEvent += HandleDropOutEvent;
	}

	private void HandleDropOutEvent(int playerIndex)
	{
		_playerCount -= 1;
	}

	private void HandlePlayerReadyEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		_playerCount += 1;
	}

	private void HandlePlayerKillEvent(ZMPlayerController killer)
	{
		int killerIndex = (int) killer.PlayerInfo.playerTag;
		
		Settings.LobbyKillcount.value[killerIndex] += 1;
	}
}
