using UnityEngine;
using System.Collections;

public class ZMBreakable : MonoBehaviour {
	public ParticleSystem destructionEffect;
	
	public void HandleCollision() {
		destructionEffect.transform.position = transform.position;
		destructionEffect.Play();

		Invoke ("StopGibs", 0.1f);

		gameObject.SetActive(false);
		//renderer.enabled = false;
		//collider2D.enabled = false;
	}

	void StopGibs() {
		destructionEffect.Stop();
		Destroy(gameObject);
	}
}
