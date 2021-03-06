﻿using UnityEngine;
using Core;
using ZMPlayer;

public class ZMGoBackController : MonoBehaviour
{
	public AudioClip _audioBack;
	public AudioClip[] _audioStart;

	void Awake()
	{
		ZMGameInputManager.AnyInputEvent += HandleGoBack;
	}

	void Start()
	{
		GetComponent<AudioSource>().PlayOneShot(_audioStart[Random.Range(0, _audioStart.Length)]);
	}

	private void HandleGoBack(IntEventArgs args)
	{
		GetComponent<AudioSource>().PlayOneShot(_audioBack);
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_MAIN_MENU);
	}
}
