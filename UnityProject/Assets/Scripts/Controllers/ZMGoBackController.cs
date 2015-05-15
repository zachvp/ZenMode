using UnityEngine;
using System.Collections;
using InControl;

public class ZMGoBackController : MonoBehaviour {

	public AudioClip _audioBack;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetButtonDown("MENU_SELECT")) {
			audio.PlayOneShot(_audioBack);
			Application.LoadLevel(1);
		}*/

		for (int i = 0; i < InputManager.Devices.Count; ++i) {
			if (InputManager.Devices[i].Action1.WasPressed) {
				audio.PlayOneShot(_audioBack);
				Application.LoadLevel(1);
			}
		}
	}
}
