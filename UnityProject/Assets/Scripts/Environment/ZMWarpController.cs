using UnityEngine;
using System.Collections.Generic;

public class ZMWarpController : MonoBehaviour {
	public LayerMask triggerMask = 0;

	private List<ZMWarpVolume> _warpVolumes;
	private BoxCollider2D _collider;

	private const string kWarpVolumeTag = "WarpVolume";

	void Awake()
	{
		_warpVolumes = new List<ZMWarpVolume>();
		_collider = GetComponent<BoxCollider2D>();
	}

	public void OnTriggerEnterCC2D(Collider2D other)
	{
		if (other.CompareTag(kWarpVolumeTag))
		{
			ZMWarpVolume warpVolume = other.GetComponent<ZMWarpVolume>();
			if (!_warpVolumes.Contains(warpVolume))
			{
//				warpVolume.Warp(gameObject);
				transform.position = warpVolume.GetWarpPosition(_collider);
				
				_warpVolumes.Add(warpVolume);
			}
		}
	}

	public void OnTriggerExitCC2D(Collider2D other)
	{
		if (other.CompareTag(kWarpVolumeTag))
		{
			ZMWarpVolume warpVolume = other.GetComponent<ZMWarpVolume>();

			_warpVolumes.Remove(warpVolume);
		}
	}
}
