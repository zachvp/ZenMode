using UnityEngine;

public class ZMFudgeParentToObject : ZMPlayerItem
{
	[SerializeField] private Vector3 offset;

	protected Transform _parent;

	public override void ConfigureItemWithID(Transform parent, int id)
	{
		base.ConfigureItemWithID(parent, id);

		_parent = ZMPlayerManager.Instance.Players[id].transform;
	}

	protected virtual void Update()
	{
		if (_parent)
		{
			transform.position = _parent.position + offset;
		}
	}
}
