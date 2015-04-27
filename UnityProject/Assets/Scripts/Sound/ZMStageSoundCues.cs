using UnityEngine;
using System.Collections;

public class ZMStageSoundCues : MonoBehaviour {
	public AudioClip focusOnPlayer;
	public AudioClip matchStart;
	public AudioClip switchFocus;

	void Awake () {
		ZMLobbyPedestalController.AtPathNodeEvent += HandleAtPathNodeEvent;
		ZMLobbyPedestalController.FullPathCycleEvent  += HandleFullCycleEvent;
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void HandleStartGameEvent ()
	{
		audio.PlayOneShot(matchStart, 0.5f);
	}

	void HandleFullCycleEvent (ZMLobbyPedestalController lobbyPedestalController) {
		audio.Play();
		audio.loop = true;
	}

	void HandleAtPathNodeEvent (ZMLobbyPedestalController lobbyPedestalController) {
		audio.PlayOneShot (switchFocus);
		Invoke ("SwitchFocus", 0.4f);
	}

	void SwitchFocus() {
		audio.PlayOneShot (focusOnPlayer);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
