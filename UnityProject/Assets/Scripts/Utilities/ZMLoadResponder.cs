using UnityEngine;
using Core;

public class ZMLoadResponder : MonoBehaviour
{
	void Awake ()
	{
		ZMMainMenuController.LoadGameEvent += HandleLoadGameEvent;

		MatchStateManager.OnMatchReset += HandleResetGameEvent;
		MatchStateManager.OnMatchExit += HandleLoadGameEvent;
	}

	void Start()
	{
		gameObject.SetActive(false);
	}

	private void HandleResetGameEvent()
	{
		gameObject.SetActive(true);
	}

	private void HandleLoadGameEvent()
	{
		gameObject.SetActive(true);
	}
}
