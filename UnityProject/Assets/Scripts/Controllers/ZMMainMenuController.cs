using UnityEngine;
using System.Collections;

public class ZMMainMenuController : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;

		ZMPauseMenuController.SelectResumeEvent += HandleSelectResumeEvent;
		ZMPauseMenuController.SelectRestartEvent += HandleSelectRestartEvent;
	}

	void HandleSelectRestartEvent ()
	{
		Application.Quit();
	}

	void HandleSelectResumeEvent ()
	{
		BeginGame();
	}

	void HandleStartInputEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		BeginGame();
	}

	private void BeginGame() {
		Invoke("LoadGame", 0.5f);
	}

	void LoadGame() {
		Application.LoadLevel(2);
	}
}
