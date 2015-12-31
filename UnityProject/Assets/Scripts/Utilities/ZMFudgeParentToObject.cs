using UnityEngine;

public class ZMFudgeParentToObject : ZMPlayerItem
{
	[SerializeField] private Vector3 offset;

	protected Transform _parent;

	public override void ConfigureItemWithID(Transform parent, int id)
	{
		base.ConfigureItemWithID(parent, id);

		InitData(ZMPlayerManager.Instance.Players[_playerInfo.ID]);
	}

	protected virtual void Update()
	{
		transform.position = _parent.position + offset;
	}

	protected virtual void InitData(ZMPlayerController controller)
	{
		_parent = controller.transform;
	}
}
