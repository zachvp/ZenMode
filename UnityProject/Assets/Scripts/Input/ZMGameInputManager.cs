using UnityEngine;
using Core;
using ZMPlayer;

public class ZMGameInputManager : MonoBehaviour
{
	// Sends integers instead of PlayerInfo because control ID could be -1.
	public static EventHandler<IntEventArgs> StartInputEvent;
	public static EventHandler<IntEventArgs> AnyInputEvent;

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
		ZMInputNotifier.Instance.OnStartButton += HandleOnStartButton;
		ZMInputNotifier.Instance.OnEscapeKey   += HandleOnStartButton;
		ZMInputNotifier.Instance.OnReturnKey   += HandleOnStartButton;
		
		ZMInputNotifier.Instance.OnAnyButton   	+= HandleOnAnyButton;
		ZMInputNotifier.Instance.OnAnyKeyPressed += HandleOnAnyButton;
	}

	private void HandleOnStartButton(ZMInputEventArgs args)
	{
		var input = args.input;

		if (input.Pressed)
		{
			var outArgs = new IntEventArgs(input.ID);

			Notifier.SendEventNotification(StartInputEvent, outArgs);
		}
	}

	private void HandleOnAnyButton(ZMInputEventArgs args)
	{
		var input = args.input;

		if (input.Pressed)
		{
			var outArgs = new IntEventArgs(input.ID);

			Notifier.SendEventNotification(AnyInputEvent, outArgs);
		}
	}
}
