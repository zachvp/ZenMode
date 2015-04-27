using UnityEngine;
using System.Collections;

public class ZMStageSoundCues : MonoBehaviour {
	public AudioClip focusOnPlayer;
	public AudioClip matchStart;
	public AudioClip switchFocus;

	void Awake () {
		ZMWaypointMovement.AtPathNodeEvent += HandleAtPathNodeEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		audio.Play();
		audio.loop = true;
	}

	void HandleStartGameEvent ()
	{
		audio.PlayOneShot(matchStart, 0.5f);
	}

	void HandleAtPathNodeEvent (ZMWaypointMovement lobbyPedestalController) {
		//audio.PlayOneShot (switchFocus);
		Invoke ("SwitchFocus", 0.1f);
	}

	void SwitchFocus() {
		audio.PlayOneShot (focusOnPlayer);
	}
}
