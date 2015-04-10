using UnityEngine;
using System.Collections;

public class ZMBreakable : MonoBehaviour {

	public void HandleCollision() {
		Destroy(gameObject);
	}
}
