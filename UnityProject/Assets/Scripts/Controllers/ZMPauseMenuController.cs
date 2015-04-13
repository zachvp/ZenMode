using UnityEngine;
using UnityEngine.UI;

public class ZMPauseMenuController : MonoBehaviour {
	public Text[] menuOptions;

	private bool _active;
	private int  _selectedIndex;
	private int  _optionsSize;

	private Color _baseColor;
	private Color _selectedColor;

	public delegate void SelectResumeOption(); public static event SelectResumeOption SelectResumeEvent;
	public delegate void SelectRestartOption(); public static event SelectRestartOption SelectRestartEvent;
	public delegate void SelectQuitOption();	public static event SelectQuitOption SelectQuitEvent;

	void Awake() {
		_selectedIndex = 0;
		_baseColor 	   = menuOptions[0].color;
		_selectedColor = new Color(255, 255, 255, 255);

		ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
		ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;

		ToggleActive(false);

		_optionsSize = menuOptions.GetLength(0);
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
	}

	void HandlePauseGameEvent ()
	{
		Debug.Log("Pause");

		_selectedIndex = 0;

		ToggleActive(true);
		UpdateUI();
	}

	void HandleResumeGameEvent() {
		Debug.Log("Resume");
		ToggleActive(false);
	}

	void HandleMenuNavigationForward() {
		_selectedIndex += 1;
		_selectedIndex %= _optionsSize;

		UpdateUI();
	}

	void HandleMenuNavigationBackward() {
		_selectedIndex -= 1;
		_selectedIndex = _selectedIndex < 0 ? _optionsSize - 1 : _selectedIndex;

		UpdateUI();
	}

	void HandleMenuSelection() {
		switch(_selectedIndex) {
			case 0: { 
				if (SelectResumeEvent != null) {
					SelectResumeEvent();
				}
				break;
			}
			case 1: {
				if (SelectRestartEvent != null) {
					SelectRestartEvent();
				}
				break;
			}
			case 2: {
				if (SelectQuitEvent != null) {
					SelectQuitEvent();
				}
				break;
			}
		}
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
}
