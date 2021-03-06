﻿using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;
using ZMConfiguration;
using Core;

[RequireComponent(typeof(ZMPlayerInfo))]
public class ZMPlayerItem : MonoBehaviour
{
	protected ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
	protected ZMPlayerController _playerController;

	protected virtual void Awake()
	{
		ZMPlayerController.OnPlayerCreate += HandlePlayerCreate;
		AcceptPlayerEvents();
	}

	protected virtual void HandlePlayerCreate(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			_playerController = args.info.GetComponent<ZMPlayerController>();
		}
	}

	public virtual void ConfigureItemWithID(Transform parent, int id)
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
				
		_playerInfo.ID = id;
		_playerInfo.standardColor = Configuration.PlayerColors[id];
		_playerInfo.lightColor = Configuration.PlayerLightColors[id];

		if (_playerController == null) { _playerController = ZMPlayerManager.Instance.Players[_playerInfo.ID]; }

		if (parent != null) { transform.SetParent(parent); }

		ConfigureHierarchyColor();
	}

	public virtual void ConfigureItemWithID(int id)
	{
		ConfigureItemWithID(null, id);
	}

	private void ConfigureHierarchyColor()
	{
		var hierarchy = Utilities.GetAllInHierarchy(transform);

		for (int i = 0; i < hierarchy.Length; ++i)
		{
			var checkVisualIgnore = hierarchy[i].GetComponent<ZMIgnoreVisualConfiguration>();

			if (checkVisualIgnore == null)
			{
				ConfigureItemColor(hierarchy[i].gameObject);
			}
		}
	}

	private void ConfigureItemColor(GameObject item)
	{
		var renderer = item.GetComponent<SpriteRenderer>();
		var light = item.GetComponent<Light>();
		var graphic = item.GetComponent<Graphic>();

		if (renderer != null) { renderer.color = Utilities.GetRGB(renderer.color, _playerInfo.standardColor); }
		if (light != null) { light.color = _playerInfo.lightColor; }
		if (graphic != null) { graphic.color = _playerInfo.standardColor; }
	}

	// Override this to listen to player events.
	protected virtual void AcceptPlayerEvents() { }
}
