using UnityEngine;

public class ZMFudgeParentToObject : MonoBehaviour {
	public Transform parent;
	public Vector3 offset;

	void Update () {
		transform.position = parent.position + offset;
	}
}
