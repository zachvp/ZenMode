using UnityEngine;
using UnityEngine.UI;

public class ZMPauseOutput : MonoBehaviour {

	public Text text;

	void Awake () {
		text.enabled = false;

		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
		ZMLobbyController.ResumeGameEvent += HandleResumeGameEvent;
	}

	void HandleResumeGameEvent ()
	{
		text.enabled = false;
	}

	void HandleStartInputEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		text.text = "P" + ((int) playerTag + 1) + " PAUSED";
		text.enabled = true;
	}
}
