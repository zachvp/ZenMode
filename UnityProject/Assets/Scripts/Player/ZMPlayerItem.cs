using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;
using ZMConfiguration;
using Core;

[RequireComponent(typeof(ZMPlayerInfo))]
public class ZMPlayerItem : MonoBehaviour
{
	protected ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
	protected ZMPlayerController _playerController;

	public virtual void ConfigureItemWithID(Transform parent, int id)
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		var renderer = GetComponent<Renderer>();
		var light = GetComponent<Light>();
		var text = GetComponent<Text>();
				
		_playerInfo.ID = id;
		_playerInfo.standardColor = Configuration.PlayerColors[id];
		_playerInfo.lightColor = Configuration.PlayerLightColors[id];

		if (_playerController == null) { _playerController = ZMPlayerManager.Instance.Players[_playerInfo.ID]; }

		if (parent != null) { transform.SetParent(parent); }
		if (renderer != null) { renderer.material.color = Utilities.GetRGB(renderer.material.color, _playerInfo.standardColor); }
		if (light != null) { light.color = _playerInfo.lightColor; }
		if (text != null) { text.color = _playerInfo.standardColor; }
	}

	protected virtual void Awake()
	{
		AcceptPlayerEvents();
	}
	
	// Override this to listen to player events.
	protected virtual void AcceptPlayerEvents() { }
}
