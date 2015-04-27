using UnityEngine;
using System.Collections;

public class ZMPersistentMusicController : MonoBehaviour {
	static bool AudioBegin = false; 
	
	void Awake()
	{
		if (GameObject.FindGameObjectsWithTag ("Music").Length > 1) {
			return;
		}

		if (!AudioBegin) {
			audio.Play ();
			DontDestroyOnLoad (gameObject);
			AudioBegin = true;
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
