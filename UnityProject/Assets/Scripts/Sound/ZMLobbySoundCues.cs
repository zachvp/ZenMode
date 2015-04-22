﻿using UnityEngine;
using System.Collections;

public class ZMLobbySoundCues : MonoBehaviour {
	public AudioClip readyUpClip;
	
	void Awake () {
		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
	}

	void HandleMaxScoreReachedEvent (ZMLobbyScoreController lobbyScoreController)
	{
		audio.PlayOneShot(readyUpClip);
	}
}