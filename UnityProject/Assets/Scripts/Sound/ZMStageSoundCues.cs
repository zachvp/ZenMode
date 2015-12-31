using UnityEngine;
using System.Collections;

public class ZMStageSoundCues : MonoBehaviour {
	public AudioClip focusOnPlayer;
	public AudioClip matchStart;
	public AudioClip switchFocus;
	public AudioClip battleIntro;
	public AudioClip[] zenPop;

	// constants
	private const string kSwitchFocusMethodName		    = "SwitchFocus";
	private const string kPlayMainBattleTrackMethodName = "PlayMainBattleTrack";

	void Awake ()
	{
		ZMWaypointMovement.AtPathNodeEvent += HandleAtPathNodeEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;

		ZMGameStateController.Instance.StartGameEvent += HandleStartGameEvent;

		ZMPedestalController.DeactivateEvent += HandleDeactivateEvent;
	}

	void HandleDeactivateEvent (ZMPedestalController pedestalController)
	{
		if (ZMGameStateController.Instance.matchState == ZMGameStateController.MatchState.MATCH)
		{ 
			int index = Random.Range(0, zenPop.Length - 1);

			audio.PlayOneShot(zenPop[index]);
		}
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag("MainCamera")) {
			// start the intro
			audio.PlayOneShot(battleIntro);
			Invoke(kPlayMainBattleTrackMethodName, battleIntro.length);
		}

	}

	void HandleStartGameEvent ()
	{
		audio.PlayOneShot(matchStart, 0.5f);
	}

	void HandleAtPathNodeEvent (ZMWaypointMovement waypointMovement, int index) {
		//audio.PlayOneShot (switchFocus);
		if (waypointMovement.CompareTag("MainCamera"))
			Invoke (kSwitchFocusMethodName, 0.1f);
	}

	void SwitchFocus() {
		audio.PlayOneShot (focusOnPlayer);
	}

	private void PlayMainBattleTrack() {
		audio.Play();
		audio.loop = true;
	}
}
