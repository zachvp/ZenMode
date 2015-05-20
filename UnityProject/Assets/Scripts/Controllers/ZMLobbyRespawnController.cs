using UnityEngine;
using System.Collections;

public class ZMLobbyRespawnController : MonoBehaviour {
	public Transform[] spawnpoints;

	void Awake () {
		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
	}

	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		int index = (int) playerController.PlayerInfo.playerTag;

		playerController.DisablePlayer();

		StartCoroutine(SpawnPlayer(index, playerController));
	}

	IEnumerator SpawnPlayer(int index, ZMPlayerController playerController) {
		yield return new WaitForSeconds(1.5f);

		playerController.transform.position = spawnpoints[index].position;
		playerController.EnablePlayer();
	}
}
