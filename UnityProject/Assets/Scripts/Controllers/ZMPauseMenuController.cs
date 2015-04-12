using UnityEngine;
using System.Collections;

public class ZMPauseMenuController : MonoBehaviour {
	private bool _active;

	void Awake() {
		ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
		ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;

		ToggleActive(false);
	}

	void HandlePauseGameEvent ()
	{
		Debug.Log("Pause");
		ToggleActive(true);
	}

	void HandleResumeGameEvent() {
		Debug.Log("Resume");
		ToggleActive(false);
	}

	private void ToggleActive(bool active) {
		_active = active;
		gameObject.SetActive(_active);
	}
}
