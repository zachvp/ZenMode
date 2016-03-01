using UnityEngine;

public abstract class ZMMenuInput : ZMDirectionalInput
{
	[SerializeField] private bool _isSharedMenu;	// If true, anyone can affect input.
	[SerializeField] protected bool _startActive;

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

	protected override bool IsValidInputControl(ZMInput input)
	{
		return IsValidInputControl(input.ID);
	}

	protected bool IsValidInputControl(int id)
	{
		if (_playerInfo == null)
		{
			Debug.LogWarningFormat("MenuInput: {0}: player info is null.", name);
			return false;
		}

		return _isSharedMenu || id == _playerInfo.ID;
	}

	protected override void AcceptGamepadEvents()
	{
		base.AcceptGamepadEvents();

		var inputManager = ZMInputManager.Instance;
		
		inputManager.OnAction1 	+= HandleOnSelect;
	}

	protected override void AcceptKeyboardEvents()
	{
		base.AcceptKeyboardEvents();

		var inputManager = ZMInputManager.Instance;

		inputManager.OnEKey 	+= HandleOnSelect;
		inputManager.OnSlashKey += HandleOnSelect;
	}

	protected override void ClearGamePadEvents()
	{
		base.ClearGamePadEvents();

		var inputManager = ZMInputManager.Instance;

		inputManager.OnAction1 	-= HandleOnSelect;
	}

	protected override void ClearKeyboardEvents()
	{
		base.ClearKeyboardEvents();

		var inputManager = ZMInputManager.Instance;
		
		inputManager.OnEKey 	-= HandleOnSelect;
		inputManager.OnSlashKey -= HandleOnSelect;
	}
	
	private void HandleOnSelect(ZMInput input)
	{
		if (input.Pressed && IsValidInputControl(input) && gameObject.activeSelf)
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
