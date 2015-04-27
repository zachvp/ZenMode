using UnityEngine;
using UnityEngine.UI;

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

	private int _resumeOption  = 0;
	private int _restartOption = 1;
	private int _quitOption	   = 2;

	public delegate void SelectResumeOption();  public static event SelectResumeOption SelectResumeEvent;
	public delegate void SelectRestartOption(); public static event SelectRestartOption SelectRestartEvent;
	public delegate void SelectQuitOption();	public static event SelectQuitOption SelectQuitEvent;

	void Awake() {
		_selectedIndex = 0;
		_baseColor 	   = menuOptions[0].color;
		_selectedColor = new Color(255, 255, 255, 255);

		if (Application.loadedLevel > 2) {
			ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
			ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;
			ZMGameStateController.GameEndEvent += HandleGameEndEvent;
		}

		ZMLobbyController.PauseGameEvent += HandlePauseGameEvent;

		ToggleActive(startActive);

		_optionsSize = menuOptions.GetLength(0);
		UpdateUI();
	}

	void HandleGameEndEvent ()
	{
		Vector3 shiftedPosition = transform.position;

		shiftedPosition.y += 64.0f;
		transform.position = shiftedPosition;

		ShowMenu();

		menuOptions[0].gameObject.SetActive(false);
		menuOptions[0] = menuOptions[1];
		menuOptions[1] = menuOptions[2];

		_optionsSize -= 1;
		_resumeOption--;
		_restartOption--;
		_quitOption--;

		ToggleSelection(0, true);
	}

	void OnDestroy() {
		SelectResumeEvent  = null;
		SelectRestartEvent = null;
		SelectQuitEvent	   = null;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			HandleMenuNavigationForward();
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			HandleMenuNavigationBackward();
		} else if (Input.GetKeyDown(KeyCode.Return)) {
			HandleMenuSelection();
		}

		if (Input.GetAxisRaw("MENU_FORWARD") > 0.8 && _canCycleSelection) {
			_canCycleSelection = false;
			HandleMenuNavigationForward();
		} else if (Input.GetAxisRaw("MENU_BACKWARD") < -0.8 && _canCycleSelection) {
			_canCycleSelection = false;
			HandleMenuNavigationBackward();
		}

		if (Input.GetButtonDown("MENU_SELECT")) {
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

	void HandlePauseGameEvent () {
		Time.timeScale = 0;
		ShowMenu();

		if (Application.loadedLevel == 2) {
			menuOptions[2].gameObject.SetActive(false);
			//menuOptions[0] = menuOptions[1];
			menuOptions[1].text = "Quit To Menu";
			
			_optionsSize = 2;
			_resumeOption = 0;
			_restartOption = -1;
			_quitOption = 1;
			
			ToggleSelection(0, true);
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
		if (_selectedIndex == _resumeOption) {
			if (SelectResumeEvent != null) {
				Time.timeScale = 1.0f;
				SelectResumeEvent();
			}
		} else if (_selectedIndex == _restartOption) {
			if (SelectRestartEvent != null) {
				SelectRestartEvent();
			}
		} else if (_selectedIndex == _quitOption) {
			if (SelectQuitEvent != null) {
				Time.timeScale = 1.0f;
				Debug.Log("Quit!");
				SelectQuitEvent();
			}
		}

		ToggleActive(false);
	}

	private void ToggleSelection(int index, bool selected) {
		menuOptions[index].color = selected ? _selectedColor : _baseColor;
	}

	private void ToggleActive(bool active) {
		_active = active;
		gameObject.SetActive(_active);
	}

	private void UpdateUI() {
		for (int i = 0; i < _optionsSize; ++i) {
			if (i != _selectedIndex) {
				ToggleSelection(i, false);
			}
		}
		
		ToggleSelection(_selectedIndex, true);
	}

	private void CanCycleSelection() {
		_canCycleSelection = true;
	}

	private void ShowMenu() {
		_selectedIndex = 0;
		_canCycleSelection = true;
		
		ToggleActive(true);
		UpdateUI();
	}
}
