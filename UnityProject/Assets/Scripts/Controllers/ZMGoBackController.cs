using UnityEngine;
using System.Collections;

public class ZMGoBackController : MonoBehaviour {

	public AudioClip _audioBack;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("MENU_SELECT")) {
			audio.PlayOneShot(_audioBack);
			Application.LoadLevel(1);
		}
	}
}
