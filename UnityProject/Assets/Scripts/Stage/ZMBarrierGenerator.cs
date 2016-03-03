using UnityEngine;
using Core;

// Generates the barriers around the edge of the stage that prevent the orbs from leaving.
public class ZMBarrierGenerator : MonoBehaviour
{
	[SerializeField] private Collider2D _barrierTemplate;

	void Awake()
	{
		ZMCameraController.OnCameraStart += HandleCameraStart;
	}

	private void HandleCameraStart(Camera camera)
	{
		CreateBarriers(ZMStageInfo.Instance.StageRect);
	}

	private void CreateBarriers(Rect area)
	{
		CreateLeftBarrier(area);
		CreateRightBarrier(area);
		CreateTopBarrier(area);
		CreateBottomBarrier(area);
	}

	private void CreateLeftBarrier(Rect area)
	{
		var barrier = CreateBarrier();

		barrier.right = Vector3.right;
		barrier.position = new Vector3(area.xMin, barrier.position.y, barrier.position.z);
		barrier.localScale = new Vector3(barrier.localScale.x, area.size.y, barrier.localScale.z);
	}

	private void CreateRightBarrier(Rect area)
	{
		var barrier = CreateBarrier();

		barrier.right = -Vector3.right;
		barrier.position = new Vector3(area.xMax, barrier.position.y, barrier.position.z);
		barrier.localScale = new Vector3(barrier.localScale.x, area.size.y, barrier.localScale.z);
	}

	private void CreateTopBarrier(Rect area)
	{
		var barrier = CreateBarrier();

		barrier.right = Vector3.down;
		barrier.position = new Vector3(barrier.position.x, area.yMax, barrier.position.z);
		barrier.localScale = new Vector3(barrier.localScale.x, area.size.x, barrier.localScale.z);
	}

	private void CreateBottomBarrier(Rect area)
	{
		var barrier = CreateBarrier();

		barrier.right = -Vector3.down;
		barrier.position = new Vector3(barrier.position.x, area.yMin, barrier.position.z);
		barrier.localScale = new Vector3(barrier.localScale.x, area.size.x, barrier.localScale.z);
	}

	private Transform CreateBarrier()
	{
		var origin = Collider2D.Instantiate(_barrierTemplate, Vector3.zero, Quaternion.identity) as Collider2D;
		var barrier = origin.transform;

		barrier.transform.parent = transform;

		return barrier;
	}
}
