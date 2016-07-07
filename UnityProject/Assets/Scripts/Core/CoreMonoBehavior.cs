using UnityEngine;
using Core;

public class CoreMonoBehavior : MonoSingleton<CoreMonoBehavior>
{
	public int FrameNum { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		Utilities.Init(this);
	}

	void LateUpdate()
	{
		FrameNum++;
	}
}
