using UnityEngine;
using System.Collections;

public class ZMLoadResponder : MonoBehaviour {
	void Awake () {
		ZMMainMenuController.LoadGameEvent += HandleLoadGameEvent;
		ZMGameStateController.QuitMatchEvent += HandleLoadGameEvent;
	}

	void Start() {
		gameObject.SetActive(false);
	}

	void OnDestroy() {
		ZMMainMenuController.LoadGameEvent -= HandleLoadGameEvent;
		ZMGameStateController.QuitMatchEvent -= HandleLoadGameEvent;
	}

	void HandleLoadGameEvent()
	{
		gameObject.SetActive(true);
	}
}
