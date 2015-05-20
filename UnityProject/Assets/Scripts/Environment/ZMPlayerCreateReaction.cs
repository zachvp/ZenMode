using UnityEngine;
using ZMPlayer;

public class ZMPlayerCreateReaction : MonoBehaviour {
	ZMPlayerInfo _playerInfo;

	void Awake() {
		_playerInfo = GetComponent<ZMPlayerInfo>();
	}

	void Start() {
		if ((int) _playerInfo.playerTag > ZMPlayerManager.PlayerCount - 1) {
			if (CompareTag("CameraFocus"))
				Destroy(gameObject);
			else
				gameObject.SetActive(false);
		}
	}
}
