using UnityEngine;
using System.Collections;
using ZMPlayer;

public class ZMDeathCover : MonoBehaviour {
	ZMPlayerInfo _playerInfo;

	// Use this for initialization
	void Awake () {
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;
	}

	void HandlePlayerRespawnEvent (ZMPlayerController playerController)
	{
		if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			GetComponent<SpriteRenderer>().enabled = false;
		}
	}

	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			GetComponent<SpriteRenderer>().enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
