using UnityEngine;
using ZMPlayer;

public class ZMLobbySoundCues : MonoBehaviour
{
	public AudioClip readyUpClip;
	
	void Awake()
	{
		ZMLobbyScoreController.OnReachMaxScore += HandleMaxScoreReachedEvent;
	}

	void HandleMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		GetComponent<AudioSource>().PlayOneShot(readyUpClip);
	}
}
