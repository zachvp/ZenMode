using UnityEngine;
using System.Collections;

public class ZMMainMenuController : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		ZMGameInputManager.StartInputEvent += HandleStartInputEvent;
	}

	void HandleStartInputEvent ()
	{
		Application.LoadLevel(1);
	}
}
