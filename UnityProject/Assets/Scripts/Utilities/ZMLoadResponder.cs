using UnityEngine;
using System.Collections;

public class ZMLoadResponder : MonoBehaviour {
	void Awake () {
		ZMMainMenuController.LoadGameEvent += HandleLoadGameEvent;
	}

	void Start() {
		gameObject.SetActive(false);
	}

	void HandleLoadGameEvent()
	{
		gameObject.SetActive(true);
	}
}
