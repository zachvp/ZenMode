using UnityEngine;
using Core;

public class ZMPauseMenu : ZMTextMenu
{
	private bool _active;

	protected override void Awake()
	{
		base.Awake ();

		if (Application.loadedLevel == ZMSceneIndexList.INDEX_LOBBY) {
			ZMLobbyController.PauseGameEvent += HandlePauseGameLobbyEvent;
		}
		else if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
			ZMGameStateController.GameEndEvent += HandleGameEndEvent;
		}

		AcceptInputEvents();
	}

	void OnDestroy()
	{
		ZMGameStateController.GameEndEvent -= HandleGameEndEvent;
	}

	private void HandlePauseGameLobbyEvent(int playerIndex)
	{
		ShowMenu();
	}

	private void HandleTogglePause(int controlIndex)
	{
		if (_active)
		{
			ResumeGame();
		}
		else
		{
			PauseGame();
		}
	}

	protected override void AcceptInputEvents()
	{
		base.AcceptInputEvents();

		ZMGameInputManager.StartInputEvent += HandleTogglePause;
	}

	protected override void HandleMenuSelection()
	{
		base.HandleMenuSelection();

		ToggleActive(false);

		if (_selectedIndex == 0)
		{
			MatchStateManager.ResumeMatch();
		}
		else if (_selectedIndex == 1)
		{
			MatchStateManager.ResetMatch();
		}
		else if (_selectedIndex == 2)
		{
			MatchStateManager.ExitMatch();
		}
	}

	protected override void ToggleActive(bool active)
	{
		base.ToggleActive(active);

		_active = active;
		gameObject.SetActive(_active);
	}

	private void PauseGame()
	{
		ShowMenu();
				
		MatchStateManager.PauseMatch();
	}
	
	private void ResumeGame()
	{
		ToggleActive(false);
		MatchStateManager.ResumeMatch();
	}

	private void HandleGameEndEvent()
	{		
		HideUI();
		
		ToggleActive(false);
	}
}
