using UnityEngine;

public class ZMShrinkOrb : MonoBehaviour
{
	public float _shrinkRate = 0.04f;
	private bool _shrinking;
	private ZMPlayer.ZMPlayerInfo _playerInfo;

	void Awake ()
	{
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();
		ZMWaypointMovement.AtPathNodeEvent += HandleAtPathNodeEvent;
	}

	void Update()
	{
		if (_shrinking && Vector3.SqrMagnitude(transform.localScale) > 0)
		{
			Vector3 newScale = transform.localScale;

			newScale = Vector3.Lerp(newScale, Vector3.zero, _shrinkRate);
			GetComponent<Light>().range = Mathf.Lerp(GetComponent<Light>().range, 0, _shrinkRate);

			transform.localScale = newScale;

			if (Vector3.SqrMagnitude(transform.localScale) <= 0.2) { gameObject.SetActive(false); }
		}
	}

	private void HandleAtPathNodeEvent(ZMWaypointMovement waypointMovement, int index)
	{
		if (index - 1 == _playerInfo.ID)
		{
			SendMessage("Stop");
			_shrinking = true;
		}
	}
}
