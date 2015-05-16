using UnityEngine;
using System.Collections;

public class ZMMainMenuController : MonoBehaviour {
	// delegates
	public delegate void LoadGameAction(); public static event LoadGameAction LoadGameEvent;

	// constants
	private const int LOBBY_SCENE_INDEX = 2;
	private const int HOW_TO_PLAY_SCENE_INDEX = 4;

	// menu options
	private const int START_OPTION		 = 0;
	private const int HOW_TO_PLAY_OPTION = 1;
	private const int QUIT_OPTION 		 = 2;

	// Use this for initialization
	void Awake () {
		ZMGameInputManager.StartInputEvent	    += HandleStartInputEvent;
		ZMPauseMenuController.SelectOptionEvent += HandleSelectOptionEvent;
	}

	void OnDestroy() {
		LoadGameEvent = null;
	}

	void HandleSelectOptionEvent(int optionIndex) {
		switch(optionIndex) {
			case START_OPTION : {
				BeginGame();
				break;
			}
			case HOW_TO_PLAY_OPTION : {
				Application.LoadLevel (HOW_TO_PLAY_SCENE_INDEX);
				break;
			}
			case QUIT_OPTION : {
				QuitGame();
				break;
			}
			default : {
				Debug.Log(gameObject.name + ": ERROR: no valid menu option selected!");
				break;
			}
		}
	}

	void QuitGame ()
	{
		Application.Quit();
	}

	void HandleStartInputEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		BeginGame();
	}

	private void BeginGame() {
		Invoke("LoadGame", 0.5f);

		if (LoadGameEvent != null) {
			LoadGameEvent();
		}
	}

	void LoadGame() {
		Application.LoadLevel(LOBBY_SCENE_INDEX);
	}
}
