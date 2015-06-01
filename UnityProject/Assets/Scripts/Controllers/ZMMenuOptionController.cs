using UnityEngine;
using UnityEngine.UI;
using InControl;

public class ZMMenuOptionController : MonoBehaviour {
	public bool startActive = true;
	public Text[] menuOptions;
	public AudioClip[] _audioChoose;
	public AudioClip[] _audioHighlight;

	private bool _active;
	public int _selectedIndex = 1;
	private int  _optionsSize;

	private bool _canCycleSelection;
	private int _delayFrame = 0;
	private const int _selectionDelay = 10;

	private Color _baseColor;
	private Color _selectedColor;

	public delegate void SelectOptionAction(int optionIndex);  public static event SelectOptionAction SelectOptionEvent;

	void Awake() {
		_baseColor 	   = menuOptions[0].color;
		_selectedColor = new Color(255, 255, 255, 255);
		_optionsSize = menuOptions.Length;

		if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
			ZMGameStateController.PauseGameEvent  += HandlePauseGameEvent;
			ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;
			ZMGameStateController.GameEndEvent 	  += HandleGameEndEvent;
		} else if (Application.loadedLevel == ZMSceneIndexList.INDEX_LOBBY) {
			ZMLobbyController.PauseGameEvent 	  += HandlePauseGameLobbyEvent;
		}

		ToggleActive(startActive);
		UpdateUI();
	}

	void OnDestroy() {
		SelectOptionEvent = null;

		ZMGameStateController.PauseGameEvent  -= ShowMenu;
		ZMGameStateController.ResumeGameEvent -= HandleResumeGameEvent;
		ZMGameStateController.GameEndEvent    -= HandleGameEndEvent;
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

		if (inputDevice.Action1 && _active) {
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

	void HandlePauseGameLobbyEvent(int playerIndex) {
		ShowMenu();
	}

	void HandlePauseGameEvent() {
		ShowMenu();

		if (name.Equals("PauseMenu-PostGame")) {
			gameObject.SetActive(false);
		}
	}

	void HandleResumeGameEvent() {
		ToggleActive(false);
	}

	void HandleGameEndEvent() {
		if (name.Equals("PauseMenu-Game")) {
			gameObject.SetActive(false);
		} else {
			enabled = false;
			HideUI();
			
			gameObject.SetActive(true);
			
			Invoke("ShowMenuEnd", 2.0f);
		}
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
			audio.PlayOneShot(_audioChoose[Random.Range (0, _audioChoose.Length)], 1.0f);

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
		enabled = active;
		gameObject.SetActive(_active);
	}

	private void CanCycleSelection() {
		_canCycleSelection = true;
	}

	private void HideUI() {
		Color transparent = new Color(_baseColor.r, _baseColor.g, _baseColor.b, 0);

		foreach (Text text in menuOptions) {
			text.color = transparent;
		}
	}

	private void ShowUI() {
		foreach (Text text in menuOptions) {
			text.color = _baseColor;
		}
	}

	void ShowMenu() {
		_selectedIndex = 0;
		_canCycleSelection = true;

		ToggleSelection(_selectedIndex, true);
		ToggleActive(true);
		UpdateUI();
	}

	void ShowMenuEnd() {
		enabled = true;
		ShowUI();

		ShowMenu();
	}
}
