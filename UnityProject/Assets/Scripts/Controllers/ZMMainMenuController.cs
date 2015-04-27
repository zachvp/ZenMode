using UnityEngine;
using System.Collections;

public class ZMMainMenuController : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;

		ZMPauseMenuController.SelectResumeEvent += HandleSelectResumeEvent;
		ZMPauseMenuController.SelectRestartEvent += HandleSelectRestartEvent;
		ZMPauseMenuController.SelectQuitEvent += HandleSelectQuitEvent;
	}

	void HandleSelectRestartEvent ()
	{
		Application.LoadLevel (4);
	}

	void HandleSelectResumeEvent ()
	{
		BeginGame();
	}

	void HandleSelectQuitEvent ()
	{
		Application.Quit();
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
