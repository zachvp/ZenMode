using UnityEngine;
using System.Collections;

public class ZMZenStream : MonoBehaviour {
	public Transform parent;

	ParticleSystem _particleSystem;

	// Use this for initialization
	void Awake () {
		_particleSystem = GetComponent<ParticleSystem>();

		ZMPedestalController.ActivateEvent += HandleActivateEvent;
		ZMPedestalController.DeactivateEvent += HandleDeactivateEvent;
		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
	}

	void Start () {
		_particleSystem.renderer.sortingLayerName = "Foreground";
	}

	void FixedUpdate() {
		transform.position = parent.position;
		transform.localScale = parent.localScale;
	}

	private void HandleActivateEvent(ZMPedestalController pedestalController) {
		_particleSystem.enableEmission = true;
	}

	private void HandleDeactivateEvent(ZMPedestalController pedestalController) {
		_particleSystem.enableEmission = false;
	}

	private void HandleMaxScoreReachedEvent(ZMLobbyScoreController lobbyScoreController) {
		Destroy(gameObject);
	}
}
