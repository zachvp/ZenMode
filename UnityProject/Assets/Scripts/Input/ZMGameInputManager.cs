using UnityEngine;
using Core;
using ZMPlayer;

public class ZMGameInputManager : MonoBehaviour
{
	// Sends integers instead of PlayerInfo because control ID could be -1.
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
		ZMInputManager.Instance.OnReturnKey   += HandleOnStartButton;
		
		ZMInputManager.Instance.OnAnyButton   	+= HandleOnAnyButton;
		ZMInputManager.Instance.OnAnyKeyPressed += HandleOnAnyButton;
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
