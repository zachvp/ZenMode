using UnityEngine;
using Core;
using ZMConfiguration;

public class TestFunction : MonoBehaviour
{
	Vector3[] guiSlots = new Vector3[8];

	void Start()
	{
		var camera = GameObject.FindGameObjectWithTag(Tags.kMainCamera).GetComponent<Camera>();

	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawRay(new Vector3(33, -24, -8), -Vector3.up * 10.0f);
	}
}
