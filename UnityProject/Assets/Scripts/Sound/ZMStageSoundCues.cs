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

	private AudioSource _audio;

	void Awake()
	{
		_audio = GetComponent<AudioSource>();

		ZMWaypointMovement.AtPathNodeEvent += HandleAtPathNodeEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;

		MatchStateManager.OnMatchStart += HandleStartGameEvent;

		ZMPedestalController.OnDeactivateEvent += HandleDeactivateEvent;
	}

	private void HandleDeactivateEvent(MonoBehaviourEventArgs args)
	{
		if (MatchStateManager.IsMain())
		{ 
			int index = Random.Range(0, zenPop.Length - 1);

			_audio.PlayOneShot(zenPop[index]);
		}
	}

	private void HandleAtPathEndEvent(ZMWaypointMovementEventArgs args)
	{
		if (args.movement.CompareTag("MainCamera"))
		{
			// start the intro
			_audio.PlayOneShot(battleIntro);
			Invoke(kPlayMainBattleTrackMethodName, battleIntro.length);
		}
	}

	private void HandleStartGameEvent()
	{
		_audio.PlayOneShot(matchStart, 0.5f);
	}

	private void HandleAtPathNodeEvent(ZMWaypointMovementIntEventArgs args)
	{
		if (args.movement.CompareTag("MainCamera"))
		{
			Invoke(kSwitchFocusMethodName, 0.1f);
		}
	}

	private void SwitchFocus()
	{
		_audio.PlayOneShot(focusOnPlayer);
	}

	private void PlayMainBattleTrack()
	{
		_audio.Play();
		_audio.loop = true;
	}
}
