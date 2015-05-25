using UnityEngine;
using System.Collections;
using InControl;

public class ZMGoBackController : MonoBehaviour {
	public AudioClip _audioBack;
	public AudioClip[] _audioStart;


	void Start () {
		audio.PlayOneShot(_audioStart[Random.Range(0, _audioStart.Length)]);
	}

	void Update () {
		for (int i = 0; i < InputManager.Devices.Count; ++i) {
			if (InputManager.Devices[i].AnyButton) {
				audio.PlayOneShot(_audioBack);
				Application.LoadLevel(ZMSceneIndexList.INDEX_MAIN_MENU);
			}
		}
	}
}
