using UnityEngine;
using System.Collections;

public class ZMLoadResponder : MonoBehaviour {
	void Awake () {
		ZMMainMenuController.LoadGameEvent += HandleLoadGameEvent;

		if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
			ZMGameStateController.Instance.ResetGameEvent += HandleResetGameEvent;
			ZMGameStateController.Instance.QuitMatchEvent += HandleLoadGameEvent;
		}
	}

	void Start() {
		gameObject.SetActive(false);
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
