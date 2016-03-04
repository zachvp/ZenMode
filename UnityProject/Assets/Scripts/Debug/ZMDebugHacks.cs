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
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			ZMAudioManager.Instance.PlayOneShot("sword_1");
		}
	}

	void OnDestroy()
	{
		OnSkipIntro = null;
	}
}
