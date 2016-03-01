using UnityEngine;
using Core;

public class ZMPauseMenuLobby : ZMPauseMenu
{
	// Menu options
	private const int RESUME_OPTION   = 0;
	private const int QUIT_OPTION 	  = 1;

	protected override void HandleMenuSelection()
	{
		base.HandleMenuSelection();

		if (_selectedIndex == RESUME_OPTION)
		{
			Invoke("ResumeGame", 0.1f);
		}
		else if (_selectedIndex == QUIT_OPTION)
		{
			MatchStateManager.ExitMatch();
		}
		else
		{
			Debug.LogWarningFormat("ZMPauseMenuLobby: Unhandled selected index {0}", _selectedIndex);
		}
	}

	protected override bool IsAbleToPause()
	{
		return ZMLobbyController.Instance.IsPlayerJoined(_playerInfo.ID);
	}

	private void HandleSelectQuitEvent()
	{
		MatchStateManager.Clear();
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_MAIN_MENU);
	}
}
