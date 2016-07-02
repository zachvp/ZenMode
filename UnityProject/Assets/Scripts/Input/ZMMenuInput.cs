using UnityEngine;
using UnityEngine.UI;
using Core;

public abstract class ZMMenuInput : ZMDirectionalInput
{
	[SerializeField] private bool _isSharedMenu;	// If true, anyone can affect input.
	[SerializeField] protected bool _startActive;

	public static EventHandler<ZMMenuOption> OnSelectOption;

	protected ZMMenuOption[] _menuOptions;

	protected int _selectedIndex;

	private int  _optionsSize;
	private bool _canCycleSelection;
	private int _delayFrame;

	private const int _selectionDelay = 10;
	private const float NAVIGATION_THRESHOLD = 0.8f;
	private const string MSG_MUST_IMPLEMENT = "{0}: must implement method {1} for base class ZMMenuInput";

	protected override void Awake()
	{
		base.Awake();

		_menuOptions = new ZMMenuOption[transform.childCount];
		_optionsSize = _menuOptions.Length;
		_canCycleSelection = true;

		AcceptActivationEvents();

		if (_startActive) { AcceptInputEvents(); }
	}

	protected virtual void Start()
	{
		ConfigureMenuOptions();
		UpdateUI();
		ToggleActive(_startActive);
	}

	protected virtual void OnDestroy()
	{
		OnSelectOption = null;
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

		var inputManager = ZMInputNotifier.Instance;
		
		inputManager.OnAction1 	+= HandleOnSelect;
	}

	protected override void AcceptKeyboardEvents()
	{
		base.AcceptKeyboardEvents();

		var inputManager = ZMInputNotifier.Instance;

		inputManager.OnEKey 	+= HandleOnSelect;
		inputManager.OnSlashKey += HandleOnSelect;
	}

	protected override void ClearGamePadEvents()
	{
		base.ClearGamePadEvents();

		var inputManager = ZMInputNotifier.Instance;

		inputManager.OnAction1 	-= HandleOnSelect;
	}

	protected override void ClearKeyboardEvents()
	{
		base.ClearKeyboardEvents();

		var inputManager = ZMInputNotifier.Instance;
		
		inputManager.OnEKey 	-= HandleOnSelect;
		inputManager.OnSlashKey -= HandleOnSelect;
	}

	protected virtual void HandleMenuNavigationForward()
	{
		_selectedIndex += 1;
		_selectedIndex %= _optionsSize;

		UpdateUI();
	}

	protected virtual void HandleMenuNavigationBackward()
	{
		_selectedIndex -= 1;
		_selectedIndex = _selectedIndex < 0 ? _optionsSize - 1 : _selectedIndex;

		UpdateUI();
	}

	protected virtual void HandleMenuSelection()
	{
		Notifier.SendEventNotification(OnSelectOption, _menuOptions[_selectedIndex]);
	}

	protected virtual void AcceptActivationEvents()
	{
		Debug.LogErrorFormat(MSG_MUST_IMPLEMENT, name, "AcceptActivationEvents");
	}

	protected virtual void ClearActivationEvents()
	{
		Debug.LogErrorFormat(MSG_MUST_IMPLEMENT, name, "ClearActivationEvents");
	}
		
	protected virtual void ConfigureMenuOptions()
	{
		for (int i = 0; i < _menuOptions.Length; ++i)
		{
			var option = transform.GetChild(i).GetComponent<ZMMenuOption>();

			Debug.AssertFormat(option != null,
							   "ZMTextMenu: Child of ZMMenuInput does not have ZMMenuOption component.");

			option.Index = i;
			_menuOptions[i] = option;
		}
	}

	protected virtual void ToggleSelection(int index, bool selected)
	{
		Debug.LogErrorFormat(MSG_MUST_IMPLEMENT, name, "ToggleSelection");
	}

	// TODO; Require component of type ZMResponder and just call methods there.
	protected virtual void ToggleActive(bool active)
	{
		enabled = active;	// TODO: Uneccessary?
		gameObject.SetActive(active);
	}

	// Protected methods.
	protected void ShowMenu()
	{
		_selectedIndex = 0;

		ToggleSelection(_selectedIndex, true);
		ToggleActive(true);
		UpdateUI();
	}

	protected void HideUI()
	{
//		Color transparent = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0);

		foreach (ZMMenuOption option in _menuOptions) { option.Hide(); }
	}

	protected void ShowUI()
	{
		foreach (ZMMenuOption option in _menuOptions) { option.Show(); }
//		foreach (Text text in _menuOptions) { text.color = _baseColor; }
	}

	// Private methods.
	private void HandleOnSelect(ZMInput input)
	{
		if (input.Pressed && IsValidInputControl(input) && gameObject.activeSelf)
		{
			HandleMenuSelection();
		}
	}

	private void UpdateUI()
	{
		for (int i = 0; i < _optionsSize; ++i)
		{
			if (i != _selectedIndex) { ToggleSelection(i, false); }
		}

		ToggleSelection(_selectedIndex, true);
	}
}
