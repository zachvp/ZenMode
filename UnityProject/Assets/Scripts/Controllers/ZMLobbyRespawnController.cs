using UnityEngine;
using System.Collections;
using ZMConfiguration;
using ZMPlayer;

public class ZMLobbyRespawnController : ZMSpawnManager
{
	protected override void Awake()
	{
		base.Awake();

		_respawnDelay = Constants.LOBBY_RESPAWN_TIME;
	}

	protected override void SpawnPlayer(ZMPlayerController playerController)
	{
		playerController.Respawn(GetPlayerSpawnPosition(playerController.PlayerInfo));
	}
}
