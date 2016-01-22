using UnityEngine;
using Core;

public class ZMNextScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Screen.showCursor = false;
		SceneManager.LoadScene(Application.loadedLevel + 1);
	}
}
