using UnityEngine;
using System.Collections;

public class ZMGameInputManager : MonoBehaviour {
	public delegate void StartInputAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event StartInputAction StartInputEvent;
	public delegate void BackInputAction(); public static event BackInputAction BackInputEvent;
	public delegate void MainInputAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event MainInputAction MainInputEvent;

	void OnDestroy() {
		StartInputEvent = null;
		BackInputEvent  = null;
		MainInputEvent  = null;
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

		if (Input.GetButtonDown ("P3_START")) {
			if (StartInputEvent != null) {
				StartInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_3);
			}
		}

		if (Input.GetButtonDown("BACK")) {
			if (BackInputEvent != null) {
				BackInputEvent();
			}
		}

		if (Input.GetButtonDown("P1_JUMP")) {
			if (MainInputEvent != null) {
				MainInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_1);
			}
		}

		if (Input.GetButtonDown("P2_JUMP")) {
			if (MainInputEvent != null) {
				MainInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_2);
			}
		}

		if (Input.GetButtonDown("P3_JUMP")) {
			if (MainInputEvent != null) {
				MainInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_3);
			}
		}

		if (Input.GetButtonDown("P4_JUMP")) {
			if (MainInputEvent != null) {
				MainInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_4);
			}
		}
	}
}
