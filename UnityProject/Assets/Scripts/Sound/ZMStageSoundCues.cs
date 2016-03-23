using UnityEngine;
using Core;

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

		MatchStateManager.OnMatchStart += HandleStartGameEvent;

		ZMPedestalController.OnDeactivateEvent += HandleDeactivateEvent;
	}

	void HandleDeactivateEvent (ZMPedestalController pedestalController)
	{
		if (MatchStateManager.IsMain())
		{ 
			int index = Random.Range(0, zenPop.Length - 1);

			GetComponent<AudioSource>().PlayOneShot(zenPop[index]);
		}
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.CompareTag("MainCamera")) {
			// start the intro
			GetComponent<AudioSource>().PlayOneShot(battleIntro);
			Invoke(kPlayMainBattleTrackMethodName, battleIntro.length);
		}

	}

	void HandleStartGameEvent ()
	{
		GetComponent<AudioSource>().PlayOneShot(matchStart, 0.5f);
	}

	void HandleAtPathNodeEvent (ZMWaypointMovement waypointMovement, int index) {
		//audio.PlayOneShot (switchFocus);
		if (waypointMovement.CompareTag("MainCamera"))
			Invoke (kSwitchFocusMethodName, 0.1f);
	}

	void SwitchFocus() {
		GetComponent<AudioSource>().PlayOneShot (focusOnPlayer);
	}

	private void PlayMainBattleTrack() {
		GetComponent<AudioSource>().Play();
		GetComponent<AudioSource>().loop = true;
	}
}
