using UnityEngine;
using System.Collections;

public class ZMMainMenuController : MonoBehaviour {
	// delegates
	public delegate void LoadGameAction(); public static event LoadGameAction LoadGameEvent;

	// menu options
	private const int START_OPTION		 = 0;
	private const int HOW_TO_PLAY_OPTION = 1;
	private const int CREDITS_OPTION	 = 2;
	private const int QUIT_OPTION 		 = 3;

	// Use this for initialization
	void Awake () {
		ZMGameInputManager.StartInputEvent	     += HandleStartInputEvent;
		ZMMenuOptionController.SelectOptionEvent += HandleSelectOptionEvent;
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
				Application.LoadLevel (ZMSceneIndexList.INDEX_HOW_TO_PLAY);
				break;
			}
			case CREDITS_OPTION : {
				Application.LoadLevel(ZMSceneIndexList.INDEX_CREDITS);
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

	void QuitGame()
	{
		Application.Quit();
	}

	void HandleStartInputEvent(int controlIndex)
	{
		BeginGame();
	}

	private void BeginGame()
	{
		Invoke("LoadGame", 0.5f);

		if (LoadGameEvent != null)
		{
			LoadGameEvent();
		}
	}

	void LoadGame() {
		Application.LoadLevel(ZMSceneIndexList.INDEX_LOBBY);
	}
}
