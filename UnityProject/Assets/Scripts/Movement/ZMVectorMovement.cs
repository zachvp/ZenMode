using UnityEngine;
using System.Collections;

public class ZMVectorMovement : MonoBehaviour {
	public float speed = 1;
	public Vector3 direction;

	// Update is called once per frame
	void Update () {
		transform.Translate(speed * direction * Time.deltaTime);
	}
}
