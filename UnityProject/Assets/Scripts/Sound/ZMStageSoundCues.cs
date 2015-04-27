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
		if (waypointMovement.name.Equals("Main Camera")) {
			audio.Play();
			audio.loop = true;
		}
	}

	void HandleStartGameEvent ()
	{
		audio.PlayOneShot(matchStart, 0.5f);
	}

	void HandleAtPathNodeEvent (ZMWaypointMovement waypointMovement) {
		//audio.PlayOneShot (switchFocus);
		if (waypointMovement.name.Equals("Main Camera"))
			Invoke ("SwitchFocus", 0.1f);
	}

	void SwitchFocus() {
		audio.PlayOneShot (focusOnPlayer);
	}
}
