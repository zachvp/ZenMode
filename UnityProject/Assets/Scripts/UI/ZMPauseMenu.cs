using UnityEngine;
using Core;
using ZMPlayer;

public class ZMPauseMenu : ZMTextMenu
{
	public static EventHandler<ZMPlayerInfo> OnPlayerPauseGame;

	private bool _active;

	protected override void Awake()
	{
		base.Awake();

		if (SceneManager.CurrentSceneIndex == ZMSceneIndexList.INDEX_LOBBY) {
			ZMLobbyController.PauseGameEvent += HandlePauseGameLobbyEvent;
		}
		else if (SceneManager.CurrentSceneIndex > ZMSceneIndexList.INDEX_LOBBY) {
			MatchStateManager.OnMatchEnd += HandleGameEndEvent;
		}

		_playerInfo = GetComponent<ZMPlayerInfo>();
	}

	void OnDestroy()
	{
		OnPlayerPauseGame = null;
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
			_playerInfo.ID = controlIndex;
			PauseGame();
		}
	}

	protected override void AcceptActivationEvents()
	{
		ZMGameInputManager.StartInputEvent += HandleTogglePause;
	}

	protected override void ClearActivationEvents()
	{
		ZMGameInputManager.StartInputEvent -= HandleTogglePause;
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

		Notifier.SendEventNotification(OnPlayerPauseGame, _playerInfo);
		MatchStateManager.PauseMatch();
	}
	
	private void ResumeGame()
	{
		ToggleActive(false);
		MatchStateManager.ResumeMatch();
	}

	private void HandleGameEndEvent()
	{
		ClearInputEvents();
		HideUI();
		
		ToggleActive(false);
	}
}
