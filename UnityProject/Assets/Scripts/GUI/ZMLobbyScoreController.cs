using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZMLobbyScoreController : MonoBehaviour {
	public float maxScore = 100.0f;
	public float scoreAmount = 0.5f;
	public Slider scoreBar;

	public delegate void MaxScoreReachedAction(ZMLobbyScoreController lobbyScoreController); public static event MaxScoreReachedAction MaxScoreReachedEvent;

	// private members
	private float _currentScore;
	private bool _pedestalActive;
	private bool _readyFired;
	private bool _targetAlive;
	private ZMPlayer.ZMPlayerInfo _playerInfo; public ZMPlayer.ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	// references
	private ZMLobbyPedestalController _pedestalController;
	private Vector3 _basePosition;

	// Use this for initialization
	void Awake () {
		_currentScore = 0;
		_pedestalActive = false;
		_readyFired = false;
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		gameObject.SetActive(false);
		light.enabled = false;
		scoreBar.gameObject.SetActive(false);

		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMLobbyController.DropOutEvent += HandleDropOutEvent;
		ZMLobbyPedestalController.ActivateEvent += HandleActivateEvent;

		UpdateUI();
	}

	void HandleDropOutEvent (int playerIndex)
	{
		if (playerIndex == (int) _playerInfo.playerTag) {
			transform.position = _basePosition;
			gameObject.SetActive(false);
			scoreBar.gameObject.SetActive(false);
		}
	}

	void HandleActivateEvent (ZMLobbyPedestalController lobbyPedestalController)
	{
		if (lobbyPedestalController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			_pedestalActive = true;
			scoreBar.gameObject.SetActive(true);
		}
	}

	void Start() {
		_basePosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

		foreach (GameObject pedestal in GameObject.FindGameObjectsWithTag("Pedestal")) {
			ZMLobbyPedestalController controller = pedestal.GetComponent<ZMLobbyPedestalController>();
			
			if (controller.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				_pedestalController = controller;
			}
		}

		scoreBar.handleRect = null;
	}

	void OnDestroy() {
		MaxScoreReachedEvent = null;
	}

	void OnTriggerStay2D(Collider2D collider) {
		if (collider.gameObject.Equals(_pedestalController.gameObject)) {
			if (_currentScore < maxScore) {
				if (_pedestalActive)
					AddToScore(scoreAmount);
			} else if(!_readyFired) {
				if (MaxScoreReachedEvent != null) {
					MaxScoreReachedEvent(this);
				}

				_readyFired = true;
			}
		}
	}

	// utilities
	private void AddToScore(float amount) {
		_currentScore = Mathf.Min(_currentScore + amount, maxScore);

		UpdateUI();
	}

	private void UpdateUI() {
		_currentScore = Mathf.Max(_currentScore, 0);
		
		float normalizedScore = (_currentScore / maxScore) * 100.0f;
		
		scoreBar.value = normalizedScore; 
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (_playerInfo.playerTag.Equals(playerTag)) {
			gameObject.SetActive(true);
			light.enabled = true;

			GetComponent<ZMPlayerController>().EnablePlayer();
		}
	}
}
