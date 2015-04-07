using UnityEngine;
using System.Collections;

public class ZMThrowingKnifeController : MonoBehaviour {

	float SPEED = 3000.0f;
	bool isActive;

	void SetDirection (int direction) {
		SPEED *= direction;
	}

	void Awake () {
		rigidbody2D.velocity = new Vector2 (SPEED, 0.0f);
		isActive = false;
	}

	void FixedUpdate () {
		if (!isActive) {
			rigidbody2D.velocity = new Vector2 (SPEED, 0.0f);
		}
		if (rigidbody2D.velocity.magnitude <= 250.0f) {
			tag = "Untagged";
			gameObject.layer = LayerMask.NameToLayer("ThrowingKnives");
		}
	}

	void OnCollisionEnter2D (Collision2D coll) {
		isActive = true;
		rigidbody2D.AddTorque (Random.Range (-150, 150), ForceMode2D.Impulse);
	}
}
