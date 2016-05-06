using UnityEngine;
using Core;

[RequireComponent(typeof(Renderer))]
public class ZMSetInitialAlpha : MonoBehaviour
{
	[SerializeField] private float initialAlpha = 0.5f;

	public void Awake()
	{
		SetAlpha();
	}

	private void SetAlpha()
	{
		var renderer = GetComponent<Renderer>();
		var newColor = renderer.material.color;

		newColor.a = initialAlpha;

		renderer.material.color = newColor;
	}
}
