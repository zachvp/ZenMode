using UnityEngine;
using ZMConfiguration;
using ZMPlayer;

public class ZMLobbyPlayerManager : ZMPlayerManager
{
	private static int _playerReadyCount; public static int PlayerReadyCount { get { return _playerReadyCount; } }
	private static int _playerJoinCount; public static int PlayerJoinCount  { get { return _playerJoinCount; } }
	
	protected override void Awake()
	{
		if (debug) { _playerReadyCount = debugPlayerCount; }

		ConfigureMonoSingleton();
		InitPlayerData(Constants.MAX_PLAYERS);
		GetPlayerStartpoints();

		ZMLobbyController.PlayerReadyEvent += HandlePlayerReadyEvent;
		ZMLobbyController.DropOutEvent += HandleDropOutEvent;
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerDropIn;
	}

	// Creates the proper player-character.
	private void HandlePlayerDropIn(int id)
	{
		_players[_playerJoinCount] = CreatePlayer(_playerJoinCount);

		_playerJoinCount += 1;
	}

	private void HandleDropOutEvent(int playerIndex)
	{
		_playerJoinCount -= 1;
	}

	private void HandlePlayerReadyEvent(ZMPlayer.ZMPlayerInfo playerTag)
	{
		_playerReadyCount += 1;
	}

	private void HandlePlayerKillEvent(ZMPlayerController killer)
	{		
		Settings.LobbyKillcount.value[killer.PlayerInfo.ID] += 1;
	}
}
