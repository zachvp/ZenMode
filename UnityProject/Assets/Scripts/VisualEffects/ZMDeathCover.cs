using UnityEngine;
using System.Collections;
using ZMPlayer;

public class ZMDeathCover : MonoBehaviour
{
	ZMPlayerInfo _playerInfo;
	SpriteRenderer _spriteRenderer;

	// Use this for initialization
	void Awake ()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_spriteRenderer = GetComponent<SpriteRenderer>();

		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;
	}

	void HandlePlayerRespawnEvent (ZMPlayerController playerController)
	{
		if (_playerInfo == playerController.PlayerInfo)
		{
			_spriteRenderer.enabled = false;
		}
	}

	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		if (_playerInfo == playerController.PlayerInfo)
		{
			_spriteRenderer.enabled = true;
			gameObject.SetActive(true);
		}
	}
}
