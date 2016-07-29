using UnityEngine;
using Core;

public class ZMDirectionalInputEventNotifier : EventNotifier
{
	public EventHandler OnMoveRightEvent;
	public EventHandler OnMoveLeftEvent;
	public EventHandler OnMoveUpEvent;
	public EventHandler OnMoveDownEvent;
	public EventHandler OnNoMoveEvent;

	public EventHandler<Vector2EventArgs> OnMoveEvent;

	public string OnMoveRightString
	{
		get { return Utilities.GetVariableName(() => OnMoveRightEvent); }
	}

	public ZMDirectionalInputEventNotifier()
	{
		// Empty
	}
}

public class ZMDirectionalInput : ZMPlayerItem
{
	public ZMDirectionalInputEventNotifier _inputEventNotifier { get; private set; }
	protected Vector2 _movement;

	protected override void Awake()
	{
		base.Awake();

		_inputEventNotifier = new ZMDirectionalInputEventNotifier();
	}

	public override void ConfigureItemWithID(Transform parent, int id)
	{
		base.ConfigureItemWithID(parent, id);

		AcceptGamepadEvents();
		AcceptKeyboardEvents();
	}

	public override void ConfigureItemWithID(int id)
	{
		base.ConfigureItemWithID(id);

		AcceptGamepadEvents();
		AcceptKeyboardEvents();
	}

	// Initialization.
	protected virtual void AcceptGamepadEvents()
	{
		var inputManager = ZMInputNotifier.Instance;
		
		inputManager.OnLeftAnalogStickMove += HandleOnMove;
	}
	
	protected virtual void AcceptKeyboardEvents()
	{
		var inputManager = ZMInputNotifier.Instance;
		
		inputManager.OnAKey += HandleOnMoveLeft;
		inputManager.OnDKey += HandleOnMoveRight;
		inputManager.OnLeftArrowKey += HandleOnMoveLeft;
		inputManager.OnRightArrowKey += HandleOnMoveRight;
		
		inputManager.OnSKey += HandleOnMoveDown;
		inputManager.OnDownArrowKey += HandleOnMoveDown;
		inputManager.OnWKey += HandleOnMoveUp;
		inputManager.OnUpArrowKey += HandleOnMoveUp;
	}

	protected void AcceptInputEvents()
	{
		AcceptGamepadEvents();
		AcceptKeyboardEvents();
	}

	protected virtual void ClearGamePadEvents()
	{
		var inputManager = ZMInputNotifier.Instance;

		inputManager.OnLeftAnalogStickMove -= HandleOnMove;
	}

	protected virtual void ClearKeyboardEvents()
	{
		var inputManager = ZMInputNotifier.Instance;

		inputManager.OnAKey -= HandleOnMoveLeft;
		inputManager.OnDKey -= HandleOnMoveRight;
		inputManager.OnLeftArrowKey -= HandleOnMoveLeft;
		inputManager.OnRightArrowKey -= HandleOnMoveRight;

		inputManager.OnSKey -= HandleOnMoveDown;
		inputManager.OnDownArrowKey -= HandleOnMoveDown;
		inputManager.OnWKey -= HandleOnMoveUp;
		inputManager.OnUpArrowKey -= HandleOnMoveUp;
	}

	protected void ClearInputEvents()
	{
		ClearGamePadEvents();
		ClearKeyboardEvents();
	}

	private void HandleOnMove(ZMInputVector2EventArgs args)
	{
		if (IsValidInputControl(args.input))
		{
			var notifyArgs = new Vector2EventArgs(args.value);

			_movement = args.value;
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveEvent, notifyArgs);
		}
	}
	
	private void HandleOnMoveLeft(ZMInputEventArgs args)
	{
		var input = args.input;

		if (IsValidInputControl(input))
		{
			Vector2EventArgs notifyArgs;

			if (input.Pressed || input.Held)
			{
				_movement.x = -1.0f;
				_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveLeftEvent);
			}
			else if (input.Released)
			{
				_movement.x = 0.0f;
			}

			notifyArgs = new Vector2EventArgs(_movement);
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveEvent, notifyArgs);
		}
	}
	
	private void HandleOnMoveRight(ZMInputEventArgs args)
	{
		var input = args.input;

		if (IsValidInputControl(input))
		{
			Vector2EventArgs notifyArgs;

			if (input.Pressed || input.Held)
			{
				_movement.x = 1.0f;
				_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveRightEvent);
			}
			else if (input.Released)
			{
				_movement.x = 0.0f;
			}

			notifyArgs = new Vector2EventArgs(_movement);
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveEvent, notifyArgs);
		}
	}
	
	protected virtual void HandleOnMoveUp(ZMInputEventArgs args)
	{
		var input = args.input;

		if (IsValidInputControl(input))
		{
			Vector2EventArgs notifyArgs;

			if (input.Pressed || input.Held)
			{
				_movement.y = 1.0f;
				_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveUpEvent);
			}
			else if (input.Released)
			{
				_movement.y = 0.0f;
			}

			notifyArgs = new Vector2EventArgs(_movement);
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveEvent, notifyArgs);
		}
	}
	
	protected virtual void HandleOnMoveDown(ZMInputEventArgs args)
	{
		var input = args.input;

		if (IsValidInputControl(input))
		{
			Vector2EventArgs notifyArgs;

			if (input.Pressed || input.Held)
			{
				_movement.y = -1.0f;
				_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveDownEvent);
			}
			else if (input.Released)
			{
				_movement.y = 0.0f;
			}

			notifyArgs = new Vector2EventArgs(_movement);
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveEvent, notifyArgs);
		}
	}

	// Should be overridden if used.
	protected virtual bool IsValidInputControl(ZMInput input)
	{
		return input.ID == _playerInfo.ID;
	}
}