using UnityEngine;
using UnityEngine.UI;

public class ZMPauseMenuController : MonoBehaviour {
	public Text[] menuOptions;

	private bool _active;
	private int  _selectedIndex;
	private int  _optionsSize;

	private Color _baseColor;
	private Color _selectedColor;

	void Awake() {
		_selectedIndex = 0;
		_baseColor 	   = menuOptions[0].color;
		_selectedColor = new Color(255, 255, 255, 255);

		ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
		ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;

		ToggleActive(false);

		_optionsSize = menuOptions.GetLength(0);
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			HandleMenuNavigationForward();
		} else if (Input.GetKeyDown(KeyCode.UpArrow)) {
			HandleMenuNavigationBackward();
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
