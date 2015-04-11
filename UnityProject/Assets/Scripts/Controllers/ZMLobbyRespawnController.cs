using UnityEngine;
using System.Collections;

public class ZMLobbyRespawnController : MonoBehaviour {
	public Transform[] spawnpoints;

	void Awake () {
		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
	}

	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		int index = 0;
		if (playerController.PlayerInfo.playerTag.Equals(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_2)) {
			index = 1;
		}

		playerController.transform.position = spawnpoints[index].position;
		playerController.EnablePlayer();
	}
}
