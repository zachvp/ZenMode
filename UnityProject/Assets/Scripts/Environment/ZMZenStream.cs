using UnityEngine;
using Core;
using System.Collections;

public class ZMZenStream : MonoBehaviour {
	private ZMPlayerInfo _playerInfo;
	private ParticleSystem _particleSystem;

	// Use this for initialization
	void Awake () {
		_particleSystem = GetComponent<ParticleSystem>();
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMPedestalController.OnActivateEvent += HandleActivateEvent;
		ZMPedestalController.OnDeactivateEvent += HandleDeactivateEvent;
	}

	void Start () {
		_particleSystem.GetComponent<Renderer>().sortingLayerName = "Foreground";
	}

	private void HandleActivateEvent(MonoBehaviourEventArgs args)
	{
		var pedestalController = args.behavior as ZMPedestalController;

		if (_playerInfo == pedestalController.PlayerInfo)
		{
			var emission = _particleSystem.emission;

			emission.enabled = false;
		}
	}

	private void HandleDeactivateEvent(MonoBehaviourEventArgs args)
	{
		var pedestalController = args.behavior as ZMPedestalController;

		if (_playerInfo == pedestalController.PlayerInfo)
		{
//			_particleSystem.emission = true;
		}
	}

	private void HandleMaxScoreReachedEvent(ZMLobbyScoreController lobbyScoreController)
	{
		if (_playerInfo == lobbyScoreController.GetComponent<ZMPlayerInfo>())
		{
			if (gameObject != null) {
				Destroy(gameObject);
			}
		}
	}
}
