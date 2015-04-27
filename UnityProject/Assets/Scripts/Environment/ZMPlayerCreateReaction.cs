using UnityEngine;
using ZMPlayer;

public class ZMPlayerCreateReaction : MonoBehaviour {
	public bool activateOnJoin = true;

	ZMPlayerInfo _playerInfo;

	void Awake() {
		_playerInfo = GetComponent<ZMPlayerInfo>();
		ZMPlayerController.PlayerCreateEvent += HandlePlayerCreateEvent;
	}

	void HandlePlayerCreateEvent (ZMPlayerController playerController)
	{
		if ((int) _playerInfo.playerTag > ZMPlayerManager.NumPlayers - 1 && gameObject != null)
			Destroy(gameObject);
	}	
}
