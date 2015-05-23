using UnityEngine;
using System.Collections;
using InControl;

public class ZMGameInputManager : MonoBehaviour {
	public delegate void StartInputAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event StartInputAction StartInputEvent;
	public delegate void AnyButtonAction(ZMPlayer.ZMPlayerInfo.PlayerTag playerTag); public static event AnyButtonAction AnyInputEvent;

	void OnDestroy() {
		StartInputEvent = null;
		AnyInputEvent  = null;
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
				if (AnyInputEvent != null) {
					InputDevice device = InputManager.Devices[i];

					if (device.AnyButton || device.MenuWasPressed || device.LeftBumper || device.RightBumper || device.LeftTrigger || device.RightTrigger) {
						if (i == 0) { AnyInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_1); }
						if (i == 1) { AnyInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_2); }
						if (i == 2) { AnyInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_3); }
						if (i == 3) { AnyInputEvent(ZMPlayer.ZMPlayerInfo.PlayerTag.PLAYER_4); }
					}
				}
			}
		}
	}
}
