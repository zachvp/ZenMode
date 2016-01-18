using UnityEngine;
using ZMPlayer;

public class ZMLobbySoundCues : MonoBehaviour
{
	public AudioClip readyUpClip;
	
	void Awake()
	{
		ZMLobbyScoreController.OnMaxScoreReached += HandleMaxScoreReachedEvent;
	}

	void HandleMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		audio.PlayOneShot(readyUpClip);
	}
}
