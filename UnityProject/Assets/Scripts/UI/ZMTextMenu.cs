using UnityEngine;
using UnityEngine.UI;
using Core;

[RequireComponent(typeof(AudioSource))]
public class ZMTextMenu : ZMMenuInput
{
	[SerializeField] private AudioClip[] _audioChoose;
	[SerializeField] private AudioClip[] _audioHighlight;

	public static EventHandler<int> SelectOptionEvent;

	protected int _selectedIndex;

	private AudioSource _audio;
	private Text[] _menuOptions;
	
	private Color _baseColor;
	private Color _selectedColor;

	private int  _optionsSize;

	protected override void Awake()
	{
		base.Awake();

		_menuOptions = new Text[transform.childCount];
		_optionsSize = _menuOptions.Length;
		_selectedColor = new Color(255, 255, 255, 255);

		_audio = GetComponent<AudioSource>();

		Debug.AssertFormat(_audioHighlight.Length > 0, "ZMTextMenu: Array empty.");
		Debug.AssertFormat(_audioChoose.Length > 0, "ZMTextMenu: Array empty.");

		AcceptInputEvents();

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
		ToggleActive(_startActive);
	}

	protected override void HandleMenuNavigationForward()
	{
		_selectedIndex += 1;
		_selectedIndex %= _optionsSize;
		
		UpdateUI();

		if (_audioHighlight.Length > 0)
		{
			_audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		}
	}
	
	protected override void HandleMenuNavigationBackward()
	{
		_selectedIndex -= 1;
		_selectedIndex = _selectedIndex < 0 ? _optionsSize - 1 : _selectedIndex;
		
		UpdateUI();

		if (_audioHighlight.Length > 0)
		{
			_audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		}
	}
	
	protected override void HandleMenuSelection()
	{
		Notifier.SendEventNotification(SelectOptionEvent, _selectedIndex);

		if (_audioChoose.Length > 0)
		{
			_audio.PlayOneShot(_audioChoose[Random.Range (0, _audioChoose.Length)], 1.0f);
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

		foreach (Text text in _menuOptions) { text.color = transparent; }
	}

	protected void ShowUI()
	{
		foreach (Text text in _menuOptions) { text.color = _baseColor; }
	}

	protected void ShowMenu()
	{
		_selectedIndex = 0;

		ToggleSelection(_selectedIndex, true);
		ToggleActive(true);
		UpdateUI();
	}
}
