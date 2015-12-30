using UnityEngine;
using System.Collections;

public class ZMZenStream : MonoBehaviour {
	private ZMPlayer.ZMPlayerInfo _playerInfo;
	private ParticleSystem _particleSystem;

	// Use this for initialization
	void Awake () {
		_particleSystem = GetComponent<ParticleSystem>();
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMPedestalController.ActivateEvent += HandleActivateEvent;
		ZMPedestalController.DeactivateEvent += HandleDeactivateEvent;
	}

	void Start () {
		_particleSystem.renderer.sortingLayerName = "Foreground";
	}

	private void HandleActivateEvent(ZMPedestalController pedestalController)
	{
		if (_playerInfo == pedestalController.PlayerInfo)
		{
			_particleSystem.enableEmission = true;
		}
	}

	private void HandleDeactivateEvent(ZMPedestalController pedestalController)
	{
		if (_playerInfo == pedestalController.PlayerInfo)
		{
			_particleSystem.enableEmission = false;
		}
	}

	private void HandleMaxScoreReachedEvent(ZMLobbyScoreController lobbyScoreController)
	{
		if (_playerInfo == lobbyScoreController.GetComponent<ZMPlayer.ZMPlayerInfo>())
		{
			if (gameObject != null) {
				Destroy(gameObject);
			}
		}
	}
}
