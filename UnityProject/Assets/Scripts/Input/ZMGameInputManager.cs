using UnityEngine;
using System.Collections;

public class ZMGameInputManager : MonoBehaviour {
	public delegate void StartInputAction();
	public static event StartInputAction StartInputEvent;

	public delegate void BackInputAction();
	public static event StartInputAction BackInputEvent;

	// Use this for initialization
	void Start () {
	
	}

	void OnDestroy() {
		StartInputEvent = null;
		BackInputEvent = null;
	}

	void Update () {
		if (Input.GetButtonDown ("P1_START") || Input.GetButtonDown ("P2_START")) {
			if (StartInputEvent != null) {
				StartInputEvent();
			}
		}
		
		if (Input.GetButtonDown("Back")) {
			if (BackInputEvent != null) {
				BackInputEvent();
			}
		}
	}
}
