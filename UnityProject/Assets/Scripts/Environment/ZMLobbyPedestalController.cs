using UnityEngine;
using System.Collections;

public class ZMLobbyPedestalController : MonoBehaviour {
	// ID
	private ZMPlayer.ZMPlayerInfo _playerInfo; public ZMPlayer.ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	public delegate void ActivateAction(ZMLobbyPedestalController lobbyPedestalController); public static event ActivateAction ActivateEvent;

	// Use this for initialization
	void Awake() {
		gameObject.SetActive(false);

		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;
	}

	void OnDestroy() {
		ActivateEvent = null;
	}

	void HandleAtPathEndEvent (ZMWaypointMovement waypointMovement)
	{
		if (waypointMovement.gameObject.Equals(gameObject)) {
			if (ActivateEvent != null) {
				ActivateEvent(this);
			}
		}
	}

	void HandlePlayerJoinedEvent(int controlIndex)
	{
		if (_playerInfo.ID == controlIndex)
		{
			gameObject.SetActive(true);
		}
	}

	void HandleMaxScoreReachedEvent(ZMLobbyScoreController lobbyScoreController)
	{
		if (lobbyScoreController.GetComponent<ZMPlayer.ZMPlayerInfo>() == _playerInfo) {

			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			
			foreach (GameObject player in players)
			{
				var playerController = player.GetComponent<ZMPlayerController>();

				if (playerController.PlayerInfo == _playerInfo)
				{
					playerController.DisablePlayer();
					playerController.renderer.enabled = false;
					playerController.transform.position = new Vector3(9000.0f, 9000.0f, 9000.0f);

					break;
				}
			}

			gameObject.SetActive(false);

		}
	}
}
