using UnityEngine;
using Core;

public class ZMMainMenuController : MonoBehaviour
{
	// Delegates.
	public static EventHandler LoadGameEvent;

	// menu options
	private const int START_OPTION		 = 0;
	private const int HOW_TO_PLAY_OPTION = 1;
	private const int CREDITS_OPTION	 = 2;
	private const int QUIT_OPTION 		 = 3;

	// Use this for initialization
	void Awake ()
	{
		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
		ZMTextMenu.SelectOptionEvent += HandleSelectOptionEvent;
	}

	void OnDestroy()
	{
		LoadGameEvent = null;
	}

	private void HandleSelectOptionEvent(int optionIndex)
	{
		switch(optionIndex)
		{
			case START_OPTION :
			{
				BeginGame();
				break;
			}
			case HOW_TO_PLAY_OPTION :
			{
				SceneManager.LoadScene(ZMSceneIndexList.INDEX_HOW_TO_PLAY);
				break;
			}
			case CREDITS_OPTION :
			{
				SceneManager.LoadScene(ZMSceneIndexList.INDEX_CREDITS);
				break;
			}
			case QUIT_OPTION :
			{
				QuitGame();
				break;
			}
			default :
			{
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

	void LoadGame()
	{
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_LOBBY);
	}
}
