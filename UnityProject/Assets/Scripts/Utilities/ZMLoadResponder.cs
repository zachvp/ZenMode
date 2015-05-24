﻿using UnityEngine;
using System.Collections;

public class ZMLoadResponder : MonoBehaviour {
	void Awake () {
		ZMMainMenuController.LoadGameEvent += HandleLoadGameEvent;

		if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
			ZMGameStateController.ResetGameEvent += HandleResetGameEvent;
			ZMGameStateController.QuitMatchEvent += HandleLoadGameEvent;
		}
	}

	void Start() {
		gameObject.SetActive(false);
	}

	void OnDestroy() {
		ZMMainMenuController.LoadGameEvent -= HandleLoadGameEvent;
		ZMGameStateController.QuitMatchEvent -= HandleLoadGameEvent;
	}

	void HandleResetGameEvent ()
	{
		gameObject.SetActive(true);
	}

	void HandleLoadGameEvent()
	{
		gameObject.SetActive(true);
	}
}
