using UnityEngine;
using Core;

public class ZMDebugHacks : MonoBehaviour
{
	public static EventHandler OnSkipIntro;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			Notifier.SendEventNotification(OnSkipIntro);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			MatchStateManager.EndMatch();
		}
	}

	void OnDestroy()
	{
		OnSkipIntro = null;
	}
}
