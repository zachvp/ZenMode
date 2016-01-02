using UnityEngine;
using System.Collections;
using ZMPlayer;

public class ZMLobbyRespawnController : MonoBehaviour {
	public Transform[] spawnpoints;

	void Awake ()
	{
		var players = ZMPlayerManager.Instance.Players;

		for (int i = 0; i < players.Length; ++i)
		{
			players[i].PlayerDeathEvent += HandlePlayerDeathEvent;
		}
	}

	void HandlePlayerDeathEvent(ZMPlayerInfo info)
	{
		StartCoroutine(SpawnPlayer(info.GetComponent<ZMPlayerController>()));
	}

	IEnumerator SpawnPlayer(ZMPlayerController playerController)
	{
		yield return new WaitForSeconds(0.75f);

		playerController.transform.position = spawnpoints[playerController.PlayerInfo.ID].position;
		playerController.EnablePlayer();
	}
}
