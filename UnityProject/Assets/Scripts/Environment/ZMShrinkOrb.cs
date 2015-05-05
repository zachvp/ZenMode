using UnityEngine;
using System.Collections;

public class ZMShrinkOrb : MonoBehaviour {
	public float _shrinkRate = 0.04f;
	private bool _shrinking;
	private Vector3 _baseScale;

	private float _baseLightRange;

	private ZMPlayer.ZMPlayerInfo _playerInfo;

	void Awake () {
		_baseScale = transform.localScale;
		_baseLightRange = light.range;

		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMWaypointMovement.AtPathNodeEvent += HandleAtPathNodeEvent;
	}

	void HandleAtPathNodeEvent (ZMWaypointMovement waypointMovement, int index)
	{
		if (waypointMovement.name.Equals("Main Camera") && index - 1 == (int) _playerInfo.playerTag) {
			_shrinking = true;
		}
	}

	void Update() {
		if (_shrinking && Vector3.Magnitude(transform.localScale) > 0) {
			Vector3 newScale = transform.localScale;

			newScale = Vector3.Lerp(newScale, Vector3.zero, _shrinkRate);
			light.range = Mathf.Lerp(light.range, 0, _shrinkRate);

			transform.localScale = newScale;

			if (Vector3.SqrMagnitude(transform.localScale) < 0.4) {
				gameObject.SetActive(false);
			}
		}
	}
}
