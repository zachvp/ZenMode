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

	private void HandleSelectOption(ZMMenuOptionEventArgs args)
	{
		if (!_isActive && _option == args.option)
		{
			Activate();
			Utilities.ExecuteAfterDelay(Toggle, 0.2f);
		}
	}

	private void HandleAnyInput(IntEventArgs args)
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
