using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ZMWarpController : MonoBehaviour
{
	private BoxCollider2D _collider;

	private const string kWarpVolumeTag = "WarpVolume";

	void Awake()
	{
		_collider = GetComponent<BoxCollider2D>();
	}

	public void OnTriggerEnterCC2D(Collider2D other)
	{
		if (other.CompareTag(kWarpVolumeTag))
		{
			ZMWarpVolume warpVolume = other.GetComponent<ZMWarpVolume>();

			if (warpVolume != null) { transform.position = warpVolume.GetWarpPosition(_collider); }
		}
	}
}
