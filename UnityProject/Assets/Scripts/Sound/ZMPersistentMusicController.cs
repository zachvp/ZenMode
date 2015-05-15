using UnityEngine;
using System.Collections;

public class ZMPersistentMusicController : MonoBehaviour {
	static bool AudioBegin;
	
	void Awake()
	{
		if (GameObject.FindGameObjectsWithTag ("Music").Length > 1) {
			return;
		}

		if (!AudioBegin) {
			audio.Play ();
			AudioBegin = true;
			DontDestroyOnLoad (gameObject);
		} 
	}
	
	void Update () {
		if (Application.loadedLevelName == "PrototypeTest")
		{
			audio.Stop();
			AudioBegin = false;
		}
		
	}
}
