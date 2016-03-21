using UnityEngine;
using Core;
using ZMPlayer;

public class ZMPauseMenuLobby : ZMPauseMenu
{
	// Menu options
	private const int RESUME_OPTION   = 0;
	private const int DROP_OPTION 	  = 1;
	private const int QUIT_OPTION 	  = 2;

	protected override void HandleMenuSelection()
	{
		base.HandleMenuSelection();

		if (_selectedIndex == RESUME_OPTION)
		{
			ResumeGame();
		}
		else if (_selectedIndex == DROP_OPTION)
		{
			ZMLobbyController.Instance.PlayerDidDropOut(_playerInfo);
			ResumeGame();
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
		return ZMLobbyController.Instance.IsPlayerJoined(_playerInfo.ID) || MatchStateManager.IsPause();
	}

	private void HandleSelectQuitEvent()
	{
		MatchStateManager.ExitMatch();
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_MAIN_MENU);
	}
}
