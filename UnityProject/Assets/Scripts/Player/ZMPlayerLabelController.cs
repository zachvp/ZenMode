using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZMPlayerLabelController : MonoBehaviour {
	public Transform parent;

	private ZMPlayer.ZMPlayerInfo _playerInfo;
	private ZMPlayerController controller;
	private Text text;
	
	void Start () {
		controller = parent.GetComponent<ZMPlayerController> ();
		text = GetComponent<Text> ();
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();
	}

	void Update() {
		if (controller && text) {
			text.enabled = (int) _playerInfo.playerTag < ZMPlayerManager.PlayerCount && !controller.IsDead();
		}
	}
}
