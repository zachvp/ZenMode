using UnityEngine;
using System.Collections;

public class ZMLobbyRespawnController : MonoBehaviour {
	public Transform[] spawnpoints;

	void Awake () {
		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
	}

	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		StartCoroutine(SpawnPlayer(playerController));
	}

	IEnumerator SpawnPlayer(ZMPlayerController playerController) {
		yield return new WaitForSeconds(0.75f);

		playerController.transform.position = spawnpoints[playerController.PlayerInfo.ID].position;
		playerController.EnablePlayer();
	}
}
