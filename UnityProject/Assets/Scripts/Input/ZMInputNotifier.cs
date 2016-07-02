using UnityEngine;
using Core;
using InControl;

public class ZMInputNotifier : MonoSingleton<ZMInputNotifier>
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

	public EventHandler<ZMInput> OnLeftAnalogStickButton;
	public EventHandler<ZMInput> OnRightAnalogStickButton;
	public EventHandler<ZMInput> OnStartButton;
	public EventHandler<ZMInput> OnAnyButton;

	// Gamepad analog events.
	public EventHandler<ZMInput, Vector2> OnLeftAnalogStickMove;
	public EventHandler<ZMInput, Vector2> OnRightAnalogStickMove;
	public EventHandler<ZMInput, Vector2> OnDirectionalPadMove;

	public EventHandler<ZMInput, float> OnLeftTriggerAnalog;
	public EventHandler<ZMInput, float> OnRightTriggerAnalog;

	// Keyboard events.
	public EventHandler<ZMInput> OnWKey;
	public EventHandler<ZMInput> OnAKey;
	public EventHandler<ZMInput> OnSKey;
	public EventHandler<ZMInput> OnDKey;

	public EventHandler<ZMInput> OnEKey;
	public EventHandler<ZMInput> OnQKey;
	public EventHandler<ZMInput> OnRKey;

	public EventHandler<ZMInput> OnLeftArrowKey;
	public EventHandler<ZMInput> OnRightArrowKey;
	public EventHandler<ZMInput> OnUpArrowKey;
	public EventHandler<ZMInput> OnDownArrowKey;

	public EventHandler<ZMInput> OnSpacebarKey;
	public EventHandler<ZMInput> OnRightShiftKey;
	public EventHandler<ZMInput> OnReturnKey;
	public EventHandler<ZMInput> OnEscapeKey;

	public EventHandler<ZMInput> OnSlashKey;

	public EventHandler<ZMInput> OnAnyKeyPressed;

	// Defines the method type for all keyboard handling.
	private delegate bool KeyAction(KeyCode code);
	
	protected override void Awake()
	{
		base.Awake();

		Debug.LogFormat("devices: {0}", InputManager.Devices.Count);
	}

	void Update()
	{
		// Loop through input devices.
		for (int i = 0; i < InputManager.Devices.Count; ++i)
		{
			var device = InputManager.Devices[i];

			BroadcastDigitalGamepadEvents(device, i);
			BroadcastAnalogGamepadEvents(device, i);
		}

		// Broadcast keyboard events of all types.
		BroadcastKeyboardEvents(Input.GetKeyDown, ZMInput.State.PRESSED);
		BroadcastKeyboardEvents(Input.GetKey, ZMInput.State.HELD);
		BroadcastKeyboardEvents(Input.GetKeyUp, ZMInput.State.RELEASED);
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
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.LeftBumper, controlIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.LeftTrigger, controlIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.RightBumper, controlIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.RightTrigger, controlIndex));
		Notifier.SendEventNotification(OnAnyButton, startInput);

		Notifier.SendEventNotification(OnStartButton, startInput);
	}

	private void BroadcastAnalogGamepadEvents(InputDevice device, int controlIndex)
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
		CheckKey(KeyCode.LeftArrow, action, state, OnLeftArrowKey);
		CheckKey(KeyCode.RightArrow, action, state, OnRightArrowKey);
		CheckKey(KeyCode.UpArrow, action, state, OnUpArrowKey);
		CheckKey(KeyCode.DownArrow, action, state, OnDownArrowKey);

		CheckKey(KeyCode.W, action, state, OnWKey);
		CheckKey(KeyCode.A, action, state, OnAKey);
		CheckKey(KeyCode.S, action, state, OnSKey);
		CheckKey(KeyCode.D, action, state, OnDKey);
		CheckKey(KeyCode.E, action, state, OnEKey);
		CheckKey(KeyCode.Q, action, state, OnQKey);
		CheckKey(KeyCode.R, action, state, OnRKey);

		CheckKey(KeyCode.Space, action, state, OnSpacebarKey);
		CheckKey(KeyCode.RightShift, action, state, OnRightShiftKey);
		CheckKey(KeyCode.Return, action, state, OnReturnKey);
		CheckKey(KeyCode.Escape, action, state, OnEscapeKey);

		CheckKey(KeyCode.Slash, action, state, OnSlashKey);
	}

	private void CheckKey(KeyCode code, KeyAction action, ZMInput.State state, EventHandler<ZMInput> eventHandler)
	{
		if (action(code))
		{
			var input = GetInputForKeyCode(code, state);

			Notifier.SendEventNotification(eventHandler, input);
			Notifier.SendEventNotification(OnAnyKeyPressed, input);
		}
	}

	private int GetIDForKeyCode(KeyCode code)
	{
		for (int i = 0; i < ZMConfiguration.Configuration.KeyboardOwners.Length; ++i)
		{
			if (ZMConfiguration.Configuration.KeyboardOwners[i].Contains(code))
			{
				return i;
			}
		}

		Debug.LogWarning("ZMInputManger: Unable to find owner of KeyCode " + code);
		return -1;
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

	private ZMInput GetInputForKeyCode(KeyCode code, ZMInput.State state)
	{
		return new ZMInput(state, GetIDForKeyCode(code));
	}
}

public class ZMInput
{
	public bool Pressed  { get { return _state == State.PRESSED; } }
	public bool Released { get { return _state == State.RELEASED; } }
	public bool Held 	 { get { return _state == State.HELD; }  }
	
	public int ID { get { return _id; } }
	
	public enum State { PRESSED, RELEASED, HELD }
	
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
