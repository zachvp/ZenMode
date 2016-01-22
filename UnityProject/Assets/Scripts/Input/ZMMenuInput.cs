using UnityEngine;

public abstract class ZMMenuInput : ZMDirectionalInput
{
	public bool startActive;
	private bool _canCycleSelection;
	private int _delayFrame = 0;

	private const int _selectionDelay = 10;

	private const float NAVIGATION_THRESHOLD = 0.8f;

	protected override void Awake()
	{
		base.Awake();

		_canCycleSelection = true;
		AcceptActivationEvents();
	}

	protected virtual void Update()
	{		
		if (_canCycleSelection)
		{
			if (gameObject.activeSelf)
			{
				if (_movement.y < -NAVIGATION_THRESHOLD)
				{
					_canCycleSelection = false;
					
					HandleMenuNavigationForward();
				}
				else if (_movement.y > NAVIGATION_THRESHOLD)
				{
					_canCycleSelection = false;
					
					HandleMenuNavigationBackward();
				}
			}
		}
		else
		{
			_delayFrame += 1;
			
			if (_delayFrame > _selectionDelay)
			{
				_canCycleSelection = true;
				_delayFrame = 0;
			}
		}
	}

	protected virtual void AcceptInputEvents()
	{
		var inputManager = ZMInputManager.Instance;
		
		inputManager.OnAction1 += HandleOnSelect;
		inputManager.OnReturnKey += HandleOnSelect;
	}
	
	protected virtual void ClearInputEvents()
	{
		var inputManager = ZMInputManager.Instance;
		
		inputManager.OnAction1 -= HandleOnSelect;
		inputManager.OnReturnKey -= HandleOnSelect;
	}
	
	private void HandleOnSelect(ZMInput input)
	{
		if (input.Pressed && gameObject.activeSelf)
		{
			HandleMenuSelection();
		}
	}

	protected virtual void AcceptActivationEvents() { }
	protected virtual void ClearActivationEvents()  { }

	protected abstract void HandleMenuNavigationForward();
	protected abstract void HandleMenuNavigationBackward();
	protected abstract void HandleMenuSelection();
}
