using UnityEngine;
using System.Collections;

public class ZMGameInputManager : MonoBehaviour {
	public delegate void StartInputAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag);
	public static event StartInputAction StartInputEvent;

	public delegate void BackInputAction();
	public static event BackInputAction BackInputEvent;

	void OnDestroy() {
		StartInputEvent = null;
		BackInputEvent = null;
	}

	void Update () {
		if (Input.GetButtonDown ("P1_START")) {
			if (StartInputEvent != null) {
				StartInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_1);
			}
		}

		if (Input.GetButtonDown ("P2_START")) {
			if (StartInputEvent != null) {
				StartInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_2);
			}
		}

		if (Input.GetButtonDown("BACK")) {
			if (BackInputEvent != null) {
				BackInputEvent();
			}
		}
	}
}
