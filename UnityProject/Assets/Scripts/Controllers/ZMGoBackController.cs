using UnityEngine;
using System.Collections;
using InControl;

public class ZMGoBackController : MonoBehaviour {
	public AudioClip _audioBack;

	void Update () {
		for (int i = 0; i < InputManager.Devices.Count; ++i) {
			if (InputManager.Devices[i].AnyButton.WasPressed) {
				Debug.Log(gameObject.name + ": go back");

				audio.PlayOneShot(_audioBack);
				Application.LoadLevel(ZMSceneIndexList.INDEX_MAIN_MENU);
			}
		}
	}
}
