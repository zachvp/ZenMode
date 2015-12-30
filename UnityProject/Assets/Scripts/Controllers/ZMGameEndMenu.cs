using UnityEngine;
using Match;

public class ZMGameEndMenu : ZMTextMenu
{
	protected override void Awake ()
	{
		base.Awake();

		if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
			ZMGameStateController.GameEndEvent += HandleGameEndEvent;
		}
	}

	void OnDestroy()
	{
		ZMGameStateController.GameEndEvent -= HandleGameEndEvent;
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

		enabled = false;
		HideUI();
		
		gameObject.SetActive(true);
		
		Invoke("ShowMenuEnd", 2.0f);
	}
}
