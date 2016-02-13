using UnityEngine;
using System.Collections.Generic;

public class ZMWarpController : MonoBehaviour {
	public LayerMask triggerMask = 0;

	private enum CheckDirection { UP, DOWN, LEFT, RIGHT, NONE };
	private RaycastHit2D _check;
	private List<ZMWarpVolume> _warpVolumes;
	private const string kWarpVolumeTag = "WarpVolume";

	void Awake()
	{
		_warpVolumes = new List<ZMWarpVolume>();
	}

	public void OnTriggerEnterCC2D(Collider2D other)
	{
		if (other.CompareTag(kWarpVolumeTag))
		{
			ZMWarpVolume warpVolume = other.GetComponent<ZMWarpVolume>();
			warpVolume.Warp(gameObject);

			_warpVolumes.Add(warpVolume.Sibling);
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
