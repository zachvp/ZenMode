using UnityEngine;
using System.Collections;

public class ZMBreakable : MonoBehaviour {
	public ParticleSystem destructionEffect;

	private bool _handlingCollision;
	private ZMPlayer.ZMPlayerInfo _playerInfo;

	void Awake() {
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

	}

	public void HandleCollision(ZMPlayer.ZMPlayerInfo playerInfo) {
		if (_playerInfo.playerTag.Equals(playerInfo.playerTag)) {
			if (!_handlingCollision) {
				Break ();
			}
			_handlingCollision = true;
			//renderer.enabled = false;
			//collider2D.enabled = false;
		}
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
