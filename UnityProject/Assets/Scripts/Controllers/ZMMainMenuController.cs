using UnityEngine;
using Core;

public class ZMMainMenuController : ZMTextMenu
{
	// Events.
	public static EventHandler LoadGameEvent;

	// Menu options
	private const int START_OPTION		 = 0;
	private const int HOW_TO_PLAY_OPTION = 1;
	private const int CREDITS_OPTION	 = 2;
	private const int QUIT_OPTION 		 = 3;

	protected override void OnDestroy()
	{
		base.OnDestroy();

		LoadGameEvent = null;
	}

	protected override void AcceptActivationEvents()
	{
		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
	}

	protected override void HandleMenuSelection()
	{
		base.HandleMenuSelection();

		switch(_selectedIndex)
		{
			case START_OPTION :
			{
				BeginGame();
				break;
			}
			case HOW_TO_PLAY_OPTION :
			{
//				SceneManager.LoadScene(ZMSceneIndexList.INDEX_HOW_TO_PLAY);
				break;
			}
			case CREDITS_OPTION :
			{
				SceneManager.LoadScene(ZMSceneIndexList.INDEX_CREDITS);
				break;
			}
			case QUIT_OPTION :
			{
				SceneManager.QuitGame();
				break;
			}
			default :
			{
				Debug.Log(gameObject.name + ": ERROR: no valid menu option selected!");
				break;
			}
		}
	}

	private void HandleStartInputEvent(IntEventArgs args)
	{
		BeginGame();
	}

	private void BeginGame()
	{
		Utilities.ExecuteAfterDelay(LoadGame, 0.2f);

		Notifier.SendEventNotification(LoadGameEvent);
	}

	void LoadGame()
	{
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_LOBBY);
	}
}
