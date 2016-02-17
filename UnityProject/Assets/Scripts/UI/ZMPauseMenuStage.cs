using UnityEngine;
using Core;

public class ZMPauseMenuStage : ZMPauseMenu
{
	protected override void Awake()
	{
		base.Awake();

		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
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

	private void HandleGameEndEvent()
	{
		ClearInputEvents();
		HideUI();

		ToggleActive(false);
	}
}
