using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;
using Core;

// This is a general class for creating player-associated objects and configuring them accordingly.
public class ZMPlayerItemCreation : MonoBehaviour
{
	// The object to create.
	[SerializeField] private ZMPlayerItem template;

	void Awake()
	{
		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i)
		{
			var item = ZMPlayerItem.Instantiate(template) as ZMPlayerItem;
			var info = item.GetComponent<ZMPlayerInfo>();
			var renderer = item.GetComponent<Renderer>();
			var light = item.GetComponent<Light>();
			var text = item.GetComponent<Text>();

			item.transform.SetParent(transform);

			info.ID = i;
			info.standardColor = Configuration.PlayerColors[i];
			info.lightColor = Configuration.PlayerLightColors[i];

			if (renderer != null) { renderer.material.color = Utilities.GetRGB(renderer.material.color, info.standardColor); }
			if (light != null) { light.color = info.lightColor; }
			if (text != null) { text.color = info.standardColor; }
		}
	}
}
