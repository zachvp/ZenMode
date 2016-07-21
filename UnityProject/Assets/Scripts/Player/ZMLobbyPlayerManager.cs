using UnityEngine;
using ZMConfiguration;
using ZMPlayer;
using Core;

public class ZMLobbyPlayerManager : ZMPlayerManager
{
	public static int PlayerReadyCount { get { return _playerReadyCount; } }
	public static int PlayerJoinCount  { get { return _playerJoinCount; } }

	private static int _playerReadyCount;
	private static int _playerJoinCount;

	// Called in ZMPlayerManager Awake().
	// Allows for special lobby-only initialization.
	protected override void Init()
	{
		if (debug) { _playerReadyCount = debugPlayerCount; }

		InitPlayerData(Constants.MAX_PLAYERS);
		InitPlayerStartpoints();

		ZMLobbyController.PlayerReadyEvent += HandlePlayerReadyEvent;
		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;
		ZMLobbyController.OnPlayerJoinedEvent += HandlePlayerDropIn;
	}

	// Creates the proper player-character.
	private void HandlePlayerDropIn(IntEventArgs args)
	{
		_players[_playerJoinCount] = CreatePlayer(args.value); // _playerJoinCount

		_playerJoinCount += 1;
	}

	private void HandleDropOutEvent(ZMPlayerInfoEventArgs args)
	{
		_playerJoinCount -= 1;
	}

	private void HandlePlayerReadyEvent(ZMPlayerInfoEventArgs playerTag)
	{
		_playerReadyCount += 1;
	}

	private void HandlePlayerKillEvent(ZMPlayerController killer)
	{		
		Settings.LobbyKillcount.value[killer.PlayerInfo.ID] += 1;
	}
}
