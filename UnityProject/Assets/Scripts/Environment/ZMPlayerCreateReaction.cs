using UnityEngine;
using ZMPlayer;
using ZMConfiguration;

public class ZMPlayerCreateReaction : MonoBehaviour {
	ZMPlayerInfo _playerInfo;

	void Awake() {
		_playerInfo = GetComponent<ZMPlayerInfo>();
	}

	void Start() {
		if (_playerInfo.ID > Settings.MatchPlayerCount.value - 1)
		{
			if (CompareTag(Tags.kCameraFocusBase))
				Destroy(gameObject);
			else
				gameObject.SetActive(false);
		}
	}
}
