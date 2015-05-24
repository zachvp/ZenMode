using UnityEngine;
using System.Collections;

public class ZMNextScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Screen.showCursor = false;
		Application.LoadLevel(Application.loadedLevel + 1);
	}
}
