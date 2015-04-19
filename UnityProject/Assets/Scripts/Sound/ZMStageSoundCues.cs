using UnityEngine;
using System.Collections;

public class ZMStageSoundCues : MonoBehaviour {
	public AudioClip focusOnPlayer;
	public AudioClip matchStart;

	void Awake () {
		ZMLobbyPedestalController.AtPathNodeEvent += HandleAtPathNodeEvent;
		ZMLobbyPedestalController.FullPathCycleEvent  += HandleFullCycleEvent;
		ZMGameStateController.StartGameEvent += HandleStartGameEvent;
	}

	void HandleStartGameEvent ()
	{
		audio.PlayOneShot(matchStart);
	}

	void HandleFullCycleEvent (ZMLobbyPedestalController lobbyPedestalController)
	{
		audio.Play();
		audio.loop = true;
	}

	void HandleAtPathNodeEvent (ZMLobbyPedestalController lobbyPedestalController)
	{
		audio.PlayOneShot(focusOnPlayer);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
