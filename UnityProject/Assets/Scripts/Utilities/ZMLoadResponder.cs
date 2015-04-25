using UnityEngine;
using System.Collections;

public class ZMLoadResponder : MonoBehaviour {
	void Awake () {
		ZMPauseMenuController.SelectResumeEvent += HandleSelectResumeEvent;
		gameObject.SetActive(false);
	}

	void HandleSelectResumeEvent ()
	{
		gameObject.SetActive(true);
	}
}
