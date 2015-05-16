using UnityEngine;
using System.Collections;

public class ZMPersistentMusicController : MonoBehaviour {
	private static bool AudioBegin;
	
	void Awake()
	{
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
