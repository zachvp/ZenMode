﻿using UnityEngine;
using System.Collections;
using ZMPlayer;

public class ZMDeathCover : ZMPlayerItem
{
	private SpriteRenderer _spriteRenderer;

	// Use this for initialization
	protected override void Awake ()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();
		_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	protected override void AcceptPlayerEvents()
	{
		ZMPlayerController.OnPlayerDeath += HandlePlayerDeathEvent;
		ZMPlayerController.OnPlayerRespawn += HandlePlayerRespawnEvent;
	}

	private void HandlePlayerRespawnEvent(ZMPlayerControllerEventArgs args)
	{
		if (_playerInfo == args.controller.PlayerInfo)
		{
			_spriteRenderer.enabled = false;
		}
	}

	private void HandlePlayerDeathEvent(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			_spriteRenderer.enabled = true;
			gameObject.SetActive(true);
		}
	}
}
