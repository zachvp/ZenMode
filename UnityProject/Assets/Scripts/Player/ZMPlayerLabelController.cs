using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;

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
			text.enabled = _playerInfo.ID < Settings.MatchPlayerCount.value && !controller.IsDead();
		}
	}
}
