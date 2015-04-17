using UnityEngine;
using System.Collections;

public class ZMHandlePlayerJoin : MonoBehaviour {
	public ZMPlayer.ZMPlayerInfo.PlayerTag playerTag;

	// Use this for initialization
	void Awake () {
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (playerTag.Equals(this.playerTag)) {
			SendMessage("Break");
		}
	}
}
