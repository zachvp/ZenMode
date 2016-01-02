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
				
		_playerInfo.ID = id;
		_playerInfo.standardColor = Configuration.PlayerColors[id];
		_playerInfo.lightColor = Configuration.PlayerLightColors[id];

		if (_playerController == null) { _playerController = ZMPlayerManager.Instance.Players[_playerInfo.ID]; }

		if (parent != null) { transform.SetParent(parent); }

		ConfigureHierarchyColor();
	}

	private void ConfigureHierarchyColor()
	{
		var hierarchy = Utilities.GetAllInHierarchy(transform);

		for (int i = 0; i < hierarchy.Length; ++i)
		{
			ConfigureItemColor(hierarchy[i].gameObject);
		}
	}

	private void ConfigureItemColor(GameObject item)
	{
		var renderer = item.GetComponent<Renderer>();
		var light = item.GetComponent<Light>();
		var graphic = item.GetComponent<MaskableGraphic>();

		if (renderer != null) { renderer.material.color = Utilities.GetRGB(renderer.material.color, _playerInfo.standardColor); }
		if (light != null) { light.color = _playerInfo.lightColor; }
		if (graphic != null) { graphic.color = _playerInfo.standardColor; }
	}

	public virtual void ConfigureItemWithID(int id)
	{
		ConfigureItemWithID(null, id);
	}

	protected virtual void Awake()
	{
		AcceptPlayerEvents();
	}
	
	// Override this to listen to player events.
	protected virtual void AcceptPlayerEvents() { }
}
