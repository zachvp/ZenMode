using UnityEngine;
using System.Collections;
using InControl;

public class ZMGameInputManager : MonoBehaviour {
	public delegate void StartInputAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event StartInputAction StartInputEvent;
	public delegate void AnyButtonAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event AnyButtonAction AnyButtonEvent;

	void OnDestroy() {
		StartInputEvent = null;
		AnyButtonEvent  = null;
	}

	void Update () {
		if (InputManager.Devices != null) {
			for (int i = 0; i < InputManager.Devices.Count; i++) {
				// Broadcast start input.
				if (InputManager.Devices[i].MenuWasPressed && StartInputEvent != null) {
					if (i == 0) { StartInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_1); }
					if (i == 1) { StartInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_2); }
					if (i == 2) { StartInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_3); }
					if (i == 3) { StartInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_4); }
				}

				// Broadcast A button input.
				if (InputManager.Devices[i].AnyButton && AnyButtonEvent != null) {
					if (i == 0) { AnyButtonEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_1); }
					if (i == 1) { AnyButtonEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_2); }
					if (i == 2) { AnyButtonEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_3); }
					if (i == 3) { AnyButtonEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_4); }
				}
			}
		}
	}
}
