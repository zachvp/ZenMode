using UnityEngine;
using ZMPlayer;

public class ZMLobbySoundCues : MonoBehaviour
{
	public AudioClip readyUpClip;

	private AudioSource _audio;

	void Awake()
	{
		_audio = GetComponent<AudioSource>();

		ZMLobbyScoreController.OnReachMaxScore += HandleMaxScoreReachedEvent;
	}

	void HandleMaxScoreReachedEvent(ZMPlayerInfoEventArgs args)
	{
		_audio.PlayOneShot(readyUpClip);
	}
}
