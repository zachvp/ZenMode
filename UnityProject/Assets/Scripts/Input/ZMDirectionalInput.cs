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
		var inputManager = ZMInputManager.Instance;
		
		inputManager.OnLeftAnalogStickMove += HandleOnMove;
	}
	
	protected virtual void AcceptKeyboardEvents()
	{
		var inputManager = ZMInputManager.Instance;
		
		inputManager.OnAKey += HandleOnMoveLeft;
		inputManager.OnDKey += HandleOnMoveRight;
		inputManager.OnLeftArrowKey += HandleOnMoveLeft;
		inputManager.OnRightArrowKey += HandleOnMoveRight;
		
		inputManager.OnSKey += HandleOnMoveDown;
		inputManager.OnDownArrowKey += HandleOnMoveDown;
		inputManager.OnWKey += HandleOnMoveUp;
		inputManager.OnUpArrowKey += HandleOnMoveUp;
	}

	private void HandleOnMove(ZMInput input, Vector2 amount)
	{
		if (IsCorrectInputControl(input))
		{
			_movement = amount;
		}
	}
	
	private void HandleOnMoveLeft(ZMInput input)
	{
		if (IsCorrectInputControl(input))
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
		if (IsCorrectInputControl(input))
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
	
	private void HandleOnMoveUp(ZMInput input)
	{
		if (IsCorrectInputControl(input))
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
	
	private void HandleOnMoveDown(ZMInput input)
	{
		if (IsCorrectInputControl(input))
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
	protected virtual bool IsCorrectInputControl(ZMInput input) { return true; }
}