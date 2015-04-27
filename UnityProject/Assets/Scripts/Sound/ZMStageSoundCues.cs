using UnityEngine;
using System.Collections;

public class ZMStageSoundCues : MonoBehaviour {
	public AudioClip focusOnPlayer;
	public AudioClip matchStart;
	public AudioClip switchFocus;

	void Awake () {
		ZMWaypointMovement.AtPathNodeEvent += HandleAtPathNodeEvent;
		ZMWaypointMovement.FullPathCycleEvent  += HandleFullCycleEvent;
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void HandleStartGameEvent ()
	{
		audio.PlayOneShot(matchStart, 0.5f);
	}

	void HandleFullCycleEvent (ZMWaypointMovement lobbyPedestalController) {
		audio.Play();
		audio.loop = true;
	}

	void HandleAtPathNodeEvent (ZMWaypointMovement lobbyPedestalController) {
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
