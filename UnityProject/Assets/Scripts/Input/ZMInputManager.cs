using UnityEngine;
using Notifications;
using InControl;

public class ZMInputManager : MonoBehaviour
{
	// Gamepad digital events.
	public EventHandler<ZMInput> OnAction1;
	public EventHandler<ZMInput> OnAction2;
	public EventHandler<ZMInput> OnAction3;
	public EventHandler<ZMInput> OnAction4;

	public EventHandler<ZMInput> OnLeftBumper;
	public EventHandler<ZMInput> OnLeftTrigger;
	public EventHandler<ZMInput> OnRightBumper;
	public EventHandler<ZMInput> OnRightTrigger;

	// Gamepad analog events.
	public EventHandler<ZMInput, Vector2> OnLeftAnalogStickMove;
	public EventHandler<ZMInput, Vector2> OnRightAnalogStickMove;
	public EventHandler<ZMInput, Vector2> OnDirectionalPadMove;

	public EventHandler<ZMInput, float> OnLeftTriggerAnalog;
	public EventHandler<ZMInput, float> OnRightTriggerAnalog;

	public EventHandler<ZMInput> OnLeftAnalogStickButton;
	public EventHandler<ZMInput> OnRightAnalogStickButton;
	public EventHandler<ZMInput> OnStartButton;
	public EventHandler<ZMInput> OnAnyButton;

	// Keyboard events.
	public EventHandler<ZMInput> OnLeftArrowKey;
	public EventHandler<ZMInput> OnRightArrowKey;
	public EventHandler<ZMInput> OnUpArrowKey;
	public EventHandler<ZMInput> OnDownArrowKey;

	public EventHandler<ZMInput> OnWKey;
	public EventHandler<ZMInput> OnAKey;
	public EventHandler<ZMInput> OnSKey;
	public EventHandler<ZMInput> OnDKey;

	public EventHandler<ZMInput> OnEKey;
	public EventHandler<ZMInput> OnQKey;
	public EventHandler<ZMInput> OnRKey;

	public EventHandler<ZMInput> OnSpacebarKey;
	public EventHandler<ZMInput> OnReturnKey;
	public EventHandler<ZMInput> OnEscapeKey;

	// Defines the method type for all keyboard handling.
	private delegate bool KeyAction(KeyCode code);

	public static ZMInputManager Instance
	{
		get
		{
			if (_instance == null)
			{
				Debug.LogError("ZMInputManager: no instance exists in the scene.");
			}
			
			return _instance;
		}
	}

	private static ZMInputManager _instance;

	void Awake()
	{
		if (_instance != null)
		{
			Debug.LogError("ZMInputManager: More than one error exists in the scene.");
		}

		_instance = this;
	}

	void Update()
	{
		// Loop through input devices.
		for (int i = 0; i < InputManager.Devices.Count; ++i)
		{
			var device = InputManager.Devices[i];

			BroadcastDigitalGamepadEvents(device, i);
			BroadcastAnalogEvents(device, i);
		}

		// Broadcast keyboard events of all types.
		BroadcastKeyboardEvents(Input.GetKeyDown, ZMInput.State.PRESSED);
		BroadcastKeyboardEvents(Input.GetKey, ZMInput.State.HELD);
		BroadcastKeyboardEvents(Input.GetKeyUp, ZMInput.State.RELEASED);
	}

	void OnDestroy()
	{
		_instance = null;
	}

	private ZMInput.State GetStateForControl(InputControl control)
	{
		if (control.WasPressed) { return ZMInput.State.PRESSED; }
		else if (control.WasReleased) { return ZMInput.State.RELEASED; }
		else { return (ZMInput.State) (-1); }
	}

	private ZMInput GetInputForControl(InputControl control, int controlIndex)
	{
		return new ZMInput(GetStateForControl(control), controlIndex);
	}

	private void BroadcastDigitalGamepadEvents(InputDevice device, int controlIndex)
	{
		var startInput = new ZMInput(device.MenuWasPressed, controlIndex);

		Notifier.SendEventNotification(OnAction1, GetInputForControl(device.Action1, controlIndex));
		Notifier.SendEventNotification(OnAction2, GetInputForControl(device.Action2, controlIndex));
		Notifier.SendEventNotification(OnAction3, GetInputForControl(device.Action3, controlIndex));
		Notifier.SendEventNotification(OnAction4, GetInputForControl(device.Action4, controlIndex));

		Notifier.SendEventNotification(OnLeftBumper, GetInputForControl(device.LeftBumper, controlIndex));
		Notifier.SendEventNotification(OnLeftTrigger, GetInputForControl(device.LeftTrigger, controlIndex));
		Notifier.SendEventNotification(OnRightBumper, GetInputForControl(device.RightBumper, controlIndex));
		Notifier.SendEventNotification(OnRightTrigger, GetInputForControl(device.RightTrigger, controlIndex));

		Notifier.SendEventNotification(OnLeftAnalogStickButton, GetInputForControl(device.LeftStickButton, controlIndex));
		Notifier.SendEventNotification(OnRightAnalogStickButton, GetInputForControl(device.RightStickButton, controlIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.AnyButton, controlIndex));

		Notifier.SendEventNotification(OnStartButton, startInput);
	}

	private void BroadcastAnalogEvents(InputDevice device, int controlIndex)
	{
		var input = new ZMInput(ZMInput.State.HELD, controlIndex);

		Notifier.SendEventNotification(OnLeftAnalogStickMove, input, device.LeftStick.Vector);
		Notifier.SendEventNotification(OnRightAnalogStickMove, input, device.RightStick.Vector);
		Notifier.SendEventNotification(OnDirectionalPadMove, input, device.DPad.Vector);

		Notifier.SendEventNotification(OnRightTriggerAnalog, input, device.RightTrigger.Value);
		Notifier.SendEventNotification(OnLeftTriggerAnalog, input, device.LeftTrigger.Value);
	}

	private void BroadcastKeyboardEvents(KeyAction action, ZMInput.State state)
	{
		// Use garbage value of -1 as ID since we can't associate keys with a particular player right now.
		// TODO: Lookup a Keycode->PlayerID mapping.
		var input = new ZMInput(state, 1);

		if (action(KeyCode.LeftArrow))  { Notifier.SendEventNotification(OnLeftArrowKey, input); }
		if (action(KeyCode.RightArrow)) { Notifier.SendEventNotification(OnRightArrowKey, input); }
		if (action(KeyCode.UpArrow))    { Notifier.SendEventNotification(OnUpArrowKey, input); }
		if (action(KeyCode.DownArrow))  { Notifier.SendEventNotification(OnDownArrowKey, input); }

		if (action(KeyCode.W)) { Notifier.SendEventNotification(OnWKey, input); }
		if (action(KeyCode.A)) { Notifier.SendEventNotification(OnAKey, input); }
		if (action(KeyCode.S)) { Notifier.SendEventNotification(OnSKey, input); }
		if (action(KeyCode.D)) { Notifier.SendEventNotification(OnDKey, input); }
		if (action(KeyCode.E)) { Notifier.SendEventNotification(OnEKey, input); }

		if (action(KeyCode.Space))  { Notifier.SendEventNotification(OnSpacebarKey, input); }
		if (action(KeyCode.Return)) { Notifier.SendEventNotification(OnReturnKey, input); }
		if (action(KeyCode.Escape)) { Notifier.SendEventNotification(OnEscapeKey, input); }
	}
}

public class ZMInput
{
	public bool Pressed  { get { return _state == State.PRESSED; } }
	public bool Released { get { return _state == State.RELEASED; } }
	public bool Held 	 { get { return _state == State.HELD; }  }
	
	public int ID { get { return _id; } }
	
	public enum State
	{
		PRESSED,
		RELEASED,
		HELD
	}
	
	private State _state;
	private int _id;
	
	public ZMInput(State state, int id)
	{
		_state = state;
		_id = id;
	}

	public ZMInput(bool state, int id)
	{
		_state = state ? State.PRESSED : (State) (-1);
		_id = id;
	}
}
