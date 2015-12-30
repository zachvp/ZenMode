using UnityEngine;
using ZMPlayer;
using ZMConfiguration;

public class ZMPlayerCreateReaction : MonoBehaviour {
	ZMPlayerInfo _playerInfo;

	void Awake() {
		_playerInfo = GetComponent<ZMPlayerInfo>();
	}

	void Start() {
		if ((int) _playerInfo.playerTag > Settings.MatchPlayerCount.value - 1) {
			if (CompareTag("CameraFocus"))
				Destroy(gameObject);
			else
				gameObject.SetActive(false);
		}
	}
}
