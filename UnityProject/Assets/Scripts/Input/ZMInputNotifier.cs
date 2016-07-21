using UnityEngine;
using InControl;
using Core;

public class ZMInputNotifier : MonoSingleton<ZMInputNotifier>
{
	// Gamepad digital events.
	public EventHandler<ZMInputEventArgs> OnAction1;
	public EventHandler<ZMInputEventArgs> OnAction2;
	public EventHandler<ZMInputEventArgs> OnAction3;
	public EventHandler<ZMInputEventArgs> OnAction4;

	public EventHandler<ZMInputEventArgs> OnLeftBumper;
	public EventHandler<ZMInputEventArgs> OnLeftTrigger;
	public EventHandler<ZMInputEventArgs> OnRightBumper;
	public EventHandler<ZMInputEventArgs> OnRightTrigger;

	public EventHandler<ZMInputEventArgs> OnLeftAnalogStickButton;
	public EventHandler<ZMInputEventArgs> OnRightAnalogStickButton;
	public EventHandler<ZMInputEventArgs> OnStartButton;
	public EventHandler<ZMInputEventArgs> OnAnyButton;

	// Gamepad analog events.
	public EventHandler<ZMInputVector2EventArgs> OnLeftAnalogStickMove;
	public EventHandler<ZMInputVector2EventArgs> OnRightAnalogStickMove;
	public EventHandler<ZMInputVector2EventArgs> OnDirectionalPadMove;

	public EventHandler<ZMInputFloatEventArgs> OnLeftTriggerAnalog;
	public EventHandler<ZMInputFloatEventArgs> OnRightTriggerAnalog;

	// Keyboard events.
	public EventHandler<ZMInputEventArgs> OnWKey;
	public EventHandler<ZMInputEventArgs> OnAKey;
	public EventHandler<ZMInputEventArgs> OnSKey;
	public EventHandler<ZMInputEventArgs> OnDKey;

	public EventHandler<ZMInputEventArgs> OnEKey;
	public EventHandler<ZMInputEventArgs> OnQKey;
	public EventHandler<ZMInputEventArgs> OnRKey;

	public EventHandler<ZMInputEventArgs> OnLeftArrowKey;
	public EventHandler<ZMInputEventArgs> OnRightArrowKey;
	public EventHandler<ZMInputEventArgs> OnUpArrowKey;
	public EventHandler<ZMInputEventArgs> OnDownArrowKey;

	public EventHandler<ZMInputEventArgs> OnSpacebarKey;
	public EventHandler<ZMInputEventArgs> OnRightShiftKey;
	public EventHandler<ZMInputEventArgs> OnReturnKey;
	public EventHandler<ZMInputEventArgs> OnEscapeKey;

	public EventHandler<ZMInputEventArgs> OnSlashKey;

	public EventHandler<ZMInputEventArgs> OnAnyKeyPressed;

	// Defines the method type for all keyboard handling.
	private delegate bool KeyAction(KeyCode code);
	
	protected override void Awake()
	{
		base.Awake();
	}

	void Update()
	{
		// Loop through users.
		var users = ZMUserManager.Instance._users;

		for (int i = 0; i < users.Count; ++i)
		{
			var device = users[i]._device._device;

			if (device != null)
			{
				BroadcastDigitalGamepadEvents(device, i);
				BroadcastAnalogGamepadEvents(device, i);
			}
		}

		// Broadcast keyboard events of all types.
		BroadcastKeyboardEvents(Input.GetKeyDown, ZMInput.State.PRESSED);
		BroadcastKeyboardEvents(Input.GetKey, ZMInput.State.HELD);
		BroadcastKeyboardEvents(Input.GetKeyUp, ZMInput.State.RELEASED);
	}

	private void BroadcastDigitalGamepadEvents(InputDevice device, int userIndex)
	{
		var startInput = new ZMInput(device.MenuWasPressed, userIndex);
		var startArgs = new ZMInputEventArgs(startInput);

		Notifier.SendEventNotification(OnAction1, GetInputForControl(device.Action1, userIndex));
		Notifier.SendEventNotification(OnAction2, GetInputForControl(device.Action2, userIndex));
		Notifier.SendEventNotification(OnAction3, GetInputForControl(device.Action3, userIndex));
		Notifier.SendEventNotification(OnAction4, GetInputForControl(device.Action4, userIndex));

		Notifier.SendEventNotification(OnLeftBumper, GetInputForControl(device.LeftBumper, userIndex));
		Notifier.SendEventNotification(OnLeftTrigger, GetInputForControl(device.LeftTrigger, userIndex));
		Notifier.SendEventNotification(OnRightBumper, GetInputForControl(device.RightBumper, userIndex));
		Notifier.SendEventNotification(OnRightTrigger, GetInputForControl(device.RightTrigger, userIndex));

		Notifier.SendEventNotification(OnLeftAnalogStickButton, GetInputForControl(device.LeftStickButton, userIndex));
		Notifier.SendEventNotification(OnRightAnalogStickButton, GetInputForControl(device.RightStickButton, userIndex));

		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.AnyButton, userIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.LeftBumper, userIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.LeftTrigger, userIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.RightBumper, userIndex));
		Notifier.SendEventNotification(OnAnyButton, GetInputForControl(device.RightTrigger, userIndex));
		Notifier.SendEventNotification(OnAnyButton, startArgs);

		Notifier.SendEventNotification(OnStartButton, startArgs);
	}

	private void BroadcastAnalogGamepadEvents(InputDevice device, int userIndex)
	{
		var input = new ZMInput(ZMInput.State.HELD, userIndex);
		var args = new ZMInputVector2EventArgs(input, Vector2.zero);
		var floatArgs = new ZMInputFloatEventArgs(input, 0);

		args.value = device.LeftStick.Vector;
		Notifier.SendEventNotification(OnLeftAnalogStickMove, args);

		args.value = device.RightStick.Vector;
		Notifier.SendEventNotification(OnRightAnalogStickMove, args);

		args.value = device.DPad.Vector;
		Notifier.SendEventNotification(OnDirectionalPadMove, args);

		floatArgs.value = device.RightTrigger.Value;
		Notifier.SendEventNotification(OnRightTriggerAnalog, floatArgs);

		floatArgs.value = device.LeftTrigger.Value;
		Notifier.SendEventNotification(OnLeftTriggerAnalog, floatArgs);
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

	private void CheckKey(KeyCode code, KeyAction action, ZMInput.State state, EventHandler<ZMInputEventArgs> eventHandler)
	{
		if (action(code))
		{
			var input = GetInputForKeyCode(code, state);
			var args = new ZMInputEventArgs(input);

			Notifier.SendEventNotification(eventHandler, args);
			Notifier.SendEventNotification(OnAnyKeyPressed, args);
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

	private ZMInputEventArgs GetInputForControl(InputControl control, int userIndex)
	{
		var state = GetStateForControl(control);
		var input = new ZMInput(state, userIndex);
		var args = new ZMInputEventArgs(input);

		return args;
	}

	private ZMInput.State GetStateForControl(InputControl control)
	{
		if (control.WasPressed) { return ZMInput.State.PRESSED; }
		else if (control.WasReleased) { return ZMInput.State.RELEASED; }
		else { return (ZMInput.State) (-1); }
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
