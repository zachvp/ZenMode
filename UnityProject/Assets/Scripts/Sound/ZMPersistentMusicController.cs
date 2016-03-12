using UnityEngine;
using Core;

[RequireComponent(typeof(AudioSource))]
public class ZMPersistentMusicController : MonoBehaviour
{
	private static bool AudioBegin;

	private AudioSource _audio;
	
	void Awake()
	{
		_audio = GetComponent<AudioSource>();

		if (!AudioBegin)
		{
			_audio.Play();
			AudioBegin = true;
			DontDestroyOnLoad(gameObject);
		}

		SceneManager.OnLoadScene += HandleOnLoadScene;
	}

	private void HandleOnLoadScene()
	{
		if (SceneManager.CurrentSceneIndex + 1 == ZMSceneIndexList.INDEX_STAGE)
		{
			_audio.Stop();
			AudioBegin = false;
		}
	}
}
