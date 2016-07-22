using UnityEngine;
using System.Collections.Generic;
using Core;

public class ZMPlayerInputEventNotifier
{
	public EventHandler OnMoveRightEvent;
	public EventHandler OnMoveLeftEvent;
	public EventHandler OnNoMoveEvent;
	public EventHandler OnJumpEvent;
	public EventHandler OnPlungeEvent;
	public EventHandler OnParryEvent;

	public EventHandler<IntEventArgs> OnAttackEvent;

	public string OnMoveRightString
	{
		get { return Utilities.GetVariableName(() => OnMoveRightEvent); }
	}

	public ZMPlayerInputEventNotifier()
	{
		// Empty
	}

	public void TriggerEvent(EventHandler handler)
	{
		Notifier.SendEventNotification(handler);
	}

	public void TriggerEvent<T>(EventHandler<T> handler, T args)
	{
		Notifier.SendEventNotification(handler, args);
	}
}

public class ZMPlayerInputController : ZMDirectionalInput
{
	public ZMPlayerInputEventNotifier _inputEventNotifier { get; private set; }

	private const float DOT_THRESHOLD = 0.75f;

	protected override void Awake()
	{
		base.Awake();

		_inputEventNotifier = new ZMPlayerInputEventNotifier();
	}

	void Update()
	{
		var dotX = Vector2.Dot(_movement, Vector2.right);

		// Handle horizontal movement.
		if (dotX > DOT_THRESHOLD)
		{
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveRightEvent);
		}
		else if (dotX < -DOT_THRESHOLD)
		{
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnMoveLeftEvent);
		}
		else
		{
			_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnNoMoveEvent);
		}
	}

	// Initialization.
	protected override void AcceptGamepadEvents()
	{
		base.AcceptGamepadEvents();

		var inputManager = ZMInputNotifier.Instance;

		inputManager.OnAction1 += HandleOnJump;
		inputManager.OnAction3 += HandleOnJump;
		inputManager.OnAction4 += HandleOnJump;

		inputManager.OnAction2 	    += HandleOnAttack;
		inputManager.OnLeftBumper   += HandleOnAttack;
		inputManager.OnLeftTrigger  += HandleOnAttack;
		inputManager.OnRightBumper  += HandleOnAttack;
		inputManager.OnRightTrigger += HandleOnAttack;
	}

	protected override void AcceptKeyboardEvents()
	{
		base.AcceptKeyboardEvents();

		var inputManager = ZMInputNotifier.Instance;

		inputManager.OnSKey += HandleOnAttack;
		inputManager.OnEKey += HandleOnAttack;
		inputManager.OnQKey += HandleOnAttack;
		inputManager.OnRightShiftKey += HandleOnAttack;
		inputManager.OnSlashKey += HandleOnAttack;
	}

	// Handlers.
	protected override void HandleOnMoveUp(ZMInputEventArgs args)
	{
		base.HandleOnMoveUp(args);

		HandleOnJump(args);
	}

	protected override void HandleOnMoveDown(ZMInputEventArgs args)
	{
		base.HandleOnMoveDown(args);

		HandleOnAttack(args);
	}

	private void HandleOnJump(ZMInputEventArgs args)
	{
		if (IsValidInputControl(args.input))
		{
			if (args.input.Pressed)
			{
				_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnJumpEvent);
			}
		}
	}

	private void HandleOnAttack(ZMInputEventArgs args)
	{
		if (IsValidInputControl(args.input))
		{
			if (args.input.Pressed)
			{
				var dotX = Vector2.Dot(_movement, Vector2.right);
				var dotY = Vector2.Dot(_movement, Vector2.up);
				var notifyArgs = new IntEventArgs(0);

				if (dotY < -DOT_THRESHOLD)
				{
					// Excepting plunge case.
					_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnPlungeEvent);
				}
				else if (dotX > DOT_THRESHOLD)
				{
					notifyArgs.value = 1;
					_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnAttackEvent, notifyArgs);
				}
				else if (dotX < -DOT_THRESHOLD)
				{
					notifyArgs.value = -1;
					_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnAttackEvent, notifyArgs);
				}
				else
				{
					// Default lunge case.
					_inputEventNotifier.TriggerEvent(_inputEventNotifier.OnAttackEvent, notifyArgs);
				}
			}
		}
	}

	protected override bool IsValidInputControl(ZMInput input)
	{
		return input.ID == -1 || input.ID == _playerInfo.ID;
	}
}
