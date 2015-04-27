using UnityEngine;
using ZMPlayer;

public class ZMPlayerCreateReaction : MonoBehaviour {
	public bool activateOnJoin = true;

	ZMPlayerInfo _playerInfo;

	void Awake() {
		_playerInfo = GetComponent<ZMPlayerInfo>();
	}

	void Start() {
		if ((int) _playerInfo.playerTag > ZMPlayerManager.NumPlayers - 1)
			Destroy(gameObject);
	}
}
