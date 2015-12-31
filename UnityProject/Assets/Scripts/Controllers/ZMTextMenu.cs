using UnityEngine;
using UnityEngine.UI;
using Core;

public class ZMTextMenu : ZMMenuInput
{
	public bool startActive;
	public AudioClip[] _audioChoose;
	public AudioClip[] _audioHighlight;

	protected int _selectedIndex;

	private Text[] _menuOptions;
	
	private Color _baseColor;
	private Color _selectedColor;

	private int  _optionsSize;

	public static EventHandler<int> SelectOptionEvent; 

	protected override void Awake()
	{
		base.Awake();

		_menuOptions = new Text[transform.childCount];
		_optionsSize = _menuOptions.Length;
		_selectedColor = new Color(255, 255, 255, 255);

		AcceptGamepadEvents();
		AcceptKeyboardEvents();
	}

	protected void Start()
	{
		for (int i = 0; i < _menuOptions.Length; ++i)
		{
			var text = transform.GetChild(i).GetComponent<Text>();

			if (text == null) { Debug.LogError("ZMTextMenu: Child of TextMenu does not have Text component."); }

			_menuOptions[i] = text;
		}

		_baseColor = _menuOptions[0].color;

		UpdateUI();
		ToggleActive(startActive);
	}

	protected override void HandleMenuNavigationForward()
	{
		audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		_selectedIndex += 1;
		_selectedIndex %= _optionsSize;
		
		UpdateUI();
	}
	
	protected override void HandleMenuNavigationBackward()
	{
		audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		_selectedIndex -= 1;
		_selectedIndex = _selectedIndex < 0 ? _optionsSize - 1 : _selectedIndex;
		
		UpdateUI();
	}
	
	protected override void HandleMenuSelection()
	{
		audio.PlayOneShot(_audioChoose[Random.Range (0, _audioChoose.Length)], 1.0f);
		Notifier.SendEventNotification(SelectOptionEvent, _selectedIndex);
	}
	
	private void UpdateUI()
	{
		for (int i = 0; i < _optionsSize; ++i)
		{
			if (i != _selectedIndex) { ToggleSelection(i, false); }
		}
		
		ToggleSelection(_selectedIndex, true);
	}

	private void ToggleSelection(int index, bool selected)
	{
		_menuOptions[index].color = selected ? _selectedColor : _baseColor;
	}
	
	protected virtual void ToggleActive(bool active)
	{
		enabled = active;
		gameObject.SetActive(active);
	}
	
	protected void HideUI()
	{
		Color transparent = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0);

		foreach (Text text in _menuOptions) {
			text.color = transparent;
		}
	}

	protected void ShowUI()
	{
		foreach (Text text in _menuOptions)
		{
			text.color = _baseColor;
		}
	}

	protected void ShowMenu()
	{
		_selectedIndex = 0;

		ToggleSelection(_selectedIndex, true);
		ToggleActive(true);
		UpdateUI();
	}
}
