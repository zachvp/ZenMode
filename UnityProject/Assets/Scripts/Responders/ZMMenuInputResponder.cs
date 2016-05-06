using UnityEngine;
using Core;

public class ZMMenuInputResponder : ZMResponder
{
	[SerializeField] ZMMenuOption _option;

	private bool _canToggleOff;

	protected override void Awake()
	{
		base.Awake();

		ZMGameInputManager.AnyInputEvent += HandleAnyInput;
		ZMMainMenuController.OnSelectOption += HandleSelectOption;
	}

	private void HandleSelectOption(ZMMenuOption option)
	{
		if (!_isActive && _option == option)
		{
			Activate();
			Utilities.ExecuteAfterDelay(Toggle, 0.2f);
		}
	}

	private void HandleAnyInput(int id)
	{
		if (_isActive && _canToggleOff)
		{
			Deactivate();
			Toggle();
		}
	}

	private void Toggle()
	{
		_canToggleOff = !_canToggleOff;
	}
}
