using UnityEngine;
using ZMConfiguration;
using ZMPlayer;

public class ZMLobbyPlayerManager : ZMPlayerManager
{
	public static int PlayerReadyCount { get { return _playerReadyCount; } }
	public static int PlayerJoinCount  { get { return _playerJoinCount; } }

	private static int _playerReadyCount;
	private static int _playerJoinCount;
	
	protected override void Awake()
	{
		if (debug) { _playerReadyCount = debugPlayerCount; }

		InitPlayerData(Constants.MAX_PLAYERS);
		GetPlayerStartpoints();

		ZMLobbyController.PlayerReadyEvent += HandlePlayerReadyEvent;
		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;
		ZMLobbyController.OnPlayerJoinedEvent += HandlePlayerDropIn;
	}

	// Creates the proper player-character.
	private void HandlePlayerDropIn(int id)
	{
		_players[_playerJoinCount] = CreatePlayer(id); // _playerJoinCount

		_playerJoinCount += 1;
	}

	private void HandleDropOutEvent(ZMPlayerInfo info)
	{
		_playerJoinCount -= 1;
	}

	private void HandlePlayerReadyEvent(ZMPlayerInfo playerTag)
	{
		_playerReadyCount += 1;
	}

	private void HandlePlayerKillEvent(ZMPlayerController killer)
	{		
		Settings.LobbyKillcount.value[killer.PlayerInfo.ID] += 1;
	}
}
