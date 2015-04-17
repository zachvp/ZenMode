using UnityEngine;
using System.Collections;

public class ZMBreakable : MonoBehaviour {
	public ParticleSystem destructionEffect;

	private bool _handlingCollision;

	public void HandleCollision() {
		if (!_handlingCollision) {
			Break ();
		}
		_handlingCollision = true;
		//renderer.enabled = false;
		//collider2D.enabled = false;
	}

	void StopGibs() {
		destructionEffect.Stop();
		gameObject.SetActive(false);
		//Destroy(gameObject);

		_handlingCollision = false;
	}

	void Break() {
		destructionEffect = Instantiate(destructionEffect) as ParticleSystem;
		destructionEffect.transform.position = transform.position;
		destructionEffect.Play();
		
		Invoke ("StopGibs", 0.1f);
		
		gameObject.SetActive(false);
	}
}
