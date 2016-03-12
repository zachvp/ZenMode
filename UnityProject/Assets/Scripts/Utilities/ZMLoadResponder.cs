using UnityEngine;
using Core;

public class ZMLoadResponder : MonoBehaviour
{
	void Awake ()
	{
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

	private void SetActive(bool active)
	{
		gameObject.SetActive(active);
	}
}
