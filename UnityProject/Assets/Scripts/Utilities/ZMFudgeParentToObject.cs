using UnityEngine;

public class ZMFudgeParentToObject : ZMPlayerItem
{
	[SerializeField] private Vector3 offset;

	protected Transform _parent;

	protected override void Awake()
	{
		base.Awake();

		ZMPlayerController.PlayerCreateEvent += InitData;
		enabled = false;
	}

	protected virtual void Update()
	{
		transform.position = _parent.position + offset;
	}

	protected virtual void InitData(ZMPlayerController controller)
	{
		if (_playerInfo == controller.PlayerInfo)
		{
			_parent = controller.transform;
			enabled = true;
		}		
	}
}
