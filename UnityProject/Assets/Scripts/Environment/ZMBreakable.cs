using UnityEngine;
using System.Collections;

public class ZMBreakable : MonoBehaviour {
	public ParticleSystem particleSystem;
	
	public void HandleCollision() {
		particleSystem.transform.position = transform.position;
		particleSystem.Play();
		Invoke ("StopGibs", 0.1f);

		gameObject.SetActive(false);
		//renderer.enabled = false;
		//collider2D.enabled = false;
	}

	void StopGibs() {
		particleSystem.Stop();
		Destroy(gameObject);
	}
}
