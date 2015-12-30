using UnityEngine;
using System.Collections;
using InControl;
using Notifications;
using ZMPlayer;

public class ZMGameInputManager : MonoBehaviour
{
	public static EventHandler<int> StartInputEvent;
	public static EventHandler<int> AnyInputEvent;

	void Awake()
	{
		AcceptInputEvents();
	}

	void OnDestroy()
	{
		StartInputEvent = null;
		AnyInputEvent  = null;
	}

	private void AcceptInputEvents()
	{
		ZMInputManager.Instance.OnStartButton += HandleOnStartButton;
		ZMInputManager.Instance.OnEscapeKey   += HandleOnStartButton;
		
		ZMInputManager.Instance.OnAnyButton   += HandleOnAnyButton;
	}
	
	private void ClearInputEvents()
	{
		ZMInputManager.Instance.OnStartButton -= HandleOnStartButton;
		ZMInputManager.Instance.OnEscapeKey   -= HandleOnStartButton;
		
		ZMInputManager.Instance.OnAnyButton   -= HandleOnAnyButton;
	}

	private void HandleOnStartButton(ZMInput input)
	{
		if (input.Pressed)
		{
			Notifier.SendEventNotification(StartInputEvent, input.ID);
		}
	}

	private void HandleOnAnyButton(ZMInput input)
	{
		if (input.Pressed)
		{
			Notifier.SendEventNotification(AnyInputEvent, input.ID);
		}
	}
}
