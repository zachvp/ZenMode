using UnityEngine;
using UnityEngine.UI;

public class ZMDisableRenderer : MonoBehaviour
{
	void Awake()
	{
		var renderer = GetComponent<Renderer>();
		var graphic = GetComponent<MaskableGraphic>();

		if (renderer != null) { renderer.enabled = false; }
		if (graphic != null) { graphic.enabled = false; }
	}
}
