using UnityEngine;
using Core;

public class ZMLoadResponder : ZMResponder
{
	protected override void Awake()
	{
		base.Awake();

		ZMMainMenuController.LoadGameEvent += HandleLoadGameEvent;

		MatchStateManager.OnMatchReset += HandleResetGameEvent;
		MatchStateManager.OnMatchExit += HandleLoadGameEvent;

		SetActive(false);
	}

	private void HandleResetGameEvent()
	{
		SetActive(true);
	}

	private void HandleLoadGameEvent()
	{
		SetActive(true);
	}
}
