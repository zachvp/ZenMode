using UnityEngine;

public class ZMDirectionalInput : ZMPlayerItem
{
	protected Vector2 _movement;

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

	private void HandleOnMove(ZMInput input, Vector2 amount)
	{
		if (IsValidInputControl(input))
		{
			_movement = amount;
		}
	}
	
	private void HandleOnMoveLeft(ZMInput input)
	{
		if (IsValidInputControl(input))
		{
			if (input.Pressed || input.Held)
			{
				_movement.x = -1.0f;
			}
			else if (input.Released)
			{
				_movement.x = 0.0f;
			}
		}
	}
	
	private void HandleOnMoveRight(ZMInput input)
	{
		if (IsValidInputControl(input))
		{
			if (input.Pressed || input.Held)
			{
				_movement.x = 1.0f;
			}
			else if (input.Released)
			{
				_movement.x = 0.0f;
			}
		}
	}
	
	protected virtual void HandleOnMoveUp(ZMInput input)
	{
		if (IsValidInputControl(input))
		{
			if (input.Pressed || input.Held)
			{
				_movement.y = 1.0f;
			}
			else if (input.Released)
			{
				_movement.y = 0.0f;
			}
		}
	}
	
	protected virtual void HandleOnMoveDown(ZMInput input)
	{
		if (IsValidInputControl(input))
		{
			if (input.Pressed || input.Held)
			{
				_movement.y = -1.0f;
			}
			else if (input.Released)
			{
				_movement.y = 0.0f;
			}
		}
	}

	// Should be overridden if used.
	protected virtual bool IsValidInputControl(ZMInput input) { return true; }
}