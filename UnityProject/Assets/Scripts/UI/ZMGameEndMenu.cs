using UnityEngine;
using Core;

public class ZMGameEndMenu : ZMTextMenu
{
	protected override void AcceptActivationEvents()
	{
		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
	}

	protected override void HandleMenuSelection()
	{
		if (_selectedIndex == 0)
		{
			MatchStateManager.ResetMatch();
		}
		else if (_selectedIndex == 1)
		{
			MatchStateManager.ExitMatch();
		}
	}

	private void HandleGameEndEvent()
	{
		AcceptInputEvents();
		ShowMenu();
	}
}
