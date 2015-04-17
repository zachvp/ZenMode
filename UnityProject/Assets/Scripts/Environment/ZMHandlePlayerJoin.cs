using UnityEngine;
using System.Collections;

public class ZMHandlePlayerJoin : MonoBehaviour {
	public string methodAction;
	public bool sendOnce = true;

	private ZMPlayer.ZMPlayerInfo _playerInfo;
	private bool _sent;

	// Use this for initialization
	void Awake () {
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (playerTag.Equals(_playerInfo.playerTag)) {
			if (sendOnce) {
				if (!_sent) {
					SendMessage(methodAction);
					_sent = true;
				}
			} else {
				SendMessage(methodAction);
			}
		}
	}

	void Disable() {
		gameObject.SetActive(false);
	}
}
