using UnityEngine;
using UnityEngine.UI;
using Core;
using ZMPlayer;

[RequireComponent(typeof(AudioSource))]
public class ZMTextMenu : ZMMenuInput
{
	[SerializeField] private AudioClip[] _audioChoose;
	[SerializeField] private AudioClip[] _audioHighlight;

	private AudioSource _audio;
	
	private Color _baseColor;
	private Color _selectedColor;

	protected override void Awake()
	{
		base.Awake();

		_selectedColor = Color.white;

		_audio = GetComponent<AudioSource>();
		_playerInfo = GetComponent<ZMPlayerInfo>();

		Debug.AssertFormat(_audioHighlight.Length > 0, "ZMTextMenu: Array empty.");
		Debug.AssertFormat(_audioChoose.Length > 0, "ZMTextMenu: Array empty.");
	}

	protected override void ConfigureMenuOptions()
	{
		base.ConfigureMenuOptions();

		_baseColor = _menuOptions[0].graphic.color;
	}

	protected override void HandleMenuNavigationForward()
	{
		base.HandleMenuNavigationForward();
		
		if (_audioHighlight.Length > 0)
		{
			_audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		}
	}
	
	protected override void HandleMenuNavigationBackward()
	{
		base.HandleMenuNavigationBackward();

		if (_audioHighlight.Length > 0)
		{
			_audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		}
	}
	
	protected override void HandleMenuSelection()
	{
		base.HandleMenuSelection();

		if (_audioChoose.Length > 0)
		{
			_audio.PlayOneShot(_audioChoose[Random.Range (0, _audioChoose.Length)], 1.0f);
		}
	}
	
	protected override void ToggleSelection(int index, bool selected)
	{
		var graphic = _menuOptions[index].graphic;

		graphic.color = selected ? _selectedColor : _baseColor;
	}
}
