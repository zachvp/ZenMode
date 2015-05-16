using UnityEngine;
using UnityEngine.UI;
using InControl;

public class ZMPauseMenuController : MonoBehaviour {
	public bool startActive = true;
	public Text[] menuOptions;
	public AudioClip[] _audioChoose;
	public AudioClip[] _audioHighlight;

	private bool _active;
	private int  _selectedIndex;
	private int  _optionsSize;

	private bool _canCycleSelection;
	private int _delayFrame = 0;
	private const int _selectionDelay = 10;

	private Color _baseColor;
	private Color _selectedColor;

	public delegate void SelectOptionAction(int optionIndex);  public static event SelectOptionAction SelectOptionEvent;

	void Awake() {
		_selectedIndex = 0;
		_baseColor 	   = menuOptions[0].color;
		_selectedColor = new Color(255, 255, 255, 255);
		_optionsSize = menuOptions.Length;

		ZMGameStateController.PauseGameEvent  += ShowMenu;
		ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;
		ZMGameStateController.GameEndEvent 	  += ShowMenu;
		ZMLobbyController.PauseGameEvent 	  += ShowMenu;

		ToggleActive(startActive);
		UpdateUI();
	}

	void OnDestroy() {
		SelectOptionEvent = null;

		ZMGameStateController.PauseGameEvent  -= ShowMenu;
		ZMGameStateController.ResumeGameEvent -= HandleResumeGameEvent;
		ZMGameStateController.GameEndEvent    -= ShowMenu;
		ZMLobbyController.PauseGameEvent 	  -= ShowMenu;
	}

	void Update() {
		var inputDevice = InputManager.ActiveDevice;

		if (_canCycleSelection) {
			if ((inputDevice.DPadDown || inputDevice.LeftStick.Y < -0.8f)) {
				_canCycleSelection = false;

				HandleMenuNavigationForward();
			} else if ((inputDevice.DPadUp || inputDevice.LeftStick.Y > 0.8f)) {
				_canCycleSelection = false;

				HandleMenuNavigationBackward();
			}
		}

		if (inputDevice.Action1 || inputDevice.MenuWasPressed) {
			HandleMenuSelection();
		}

		if (!_canCycleSelection) {
			_delayFrame += 1;

			if (_delayFrame > _selectionDelay) {
				CanCycleSelection();
				_delayFrame = 0;
			}
		}
	}

	void HandleResumeGameEvent() {
		audio.PlayOneShot(_audioChoose[Random.Range (0, _audioChoose.Length)], 1.0f);
		ToggleActive(false);
	}

	void HandleMenuNavigationForward() {
		audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		_selectedIndex += 1;
		_selectedIndex %= _optionsSize;

		UpdateUI();
	}

	void HandleMenuNavigationBackward() {
		audio.PlayOneShot(_audioHighlight[Random.Range (0, _audioHighlight.Length)], 0.5f);
		_selectedIndex -= 1;
		_selectedIndex = _selectedIndex < 0 ? _optionsSize - 1 : _selectedIndex;

		UpdateUI();
	}

	void HandleMenuSelection() {
		if (SelectOptionEvent != null) {
			SelectOptionEvent(_selectedIndex);
		}

		ToggleActive(false);
	}

	private void UpdateUI() {
		for (int i = 0; i < _optionsSize; ++i) {
			if (i != _selectedIndex) {
				ToggleSelection(i, false);
			}
		}
		
		ToggleSelection(_selectedIndex, true);
	}

	private void ToggleSelection(int index, bool selected) {
		menuOptions[index].color = selected ? _selectedColor : _baseColor;
	}
	
	private void ToggleActive(bool active) {
		_active = active;
		gameObject.SetActive(_active);
	}

	private void CanCycleSelection() {
		_canCycleSelection = true;
	}

	private void ShowMenu() {
		_selectedIndex = 0;
		_canCycleSelection = true;

		ToggleSelection(_selectedIndex, true);
		ToggleActive(true);
		UpdateUI();
	}
}
