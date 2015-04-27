using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZMLobbyScoreController : MonoBehaviour {
	public float maxScore = 100.0f;
	public float scoreAmount = 0.5f;
	public Text scoreText;
	public Slider scoreBar;

	public delegate void MaxScoreReachedAction(ZMLobbyScoreController lobbyScoreController); public static event MaxScoreReachedAction MaxScoreReachedEvent;

	// private members
	private float _currentScore;
	private bool _readyFired;
	private ZMPlayer.ZMPlayerInfo _playerInfo; public ZMPlayer.ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	// references
	private ZMLobbyPedestalController _pedestalController;

	// Use this for initialization
	void Awake () {
		_currentScore = 0;
		_readyFired = false;
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();

		gameObject.SetActive(false);
		light.enabled = false;
		//scoreBar.gameObject.SetActive(false);

		ZMWaypointMovement.AtPathEndEvent += HandleAtPathEndEvent;
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;

		UpdateUI();
	}

	void Start() {
		foreach (GameObject pedestal in GameObject.FindGameObjectsWithTag("Pedestal")) {
			ZMLobbyPedestalController controller = pedestal.GetComponent<ZMLobbyPedestalController>();
			
			if (controller.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				_pedestalController = controller;
			}
		}
	}

	void OnDestroy() {
		MaxScoreReachedEvent = null;
	}

	void OnTriggerStay2D(Collider2D collider) {
		if (collider.gameObject.Equals(_pedestalController.gameObject)) {
			if (_currentScore < maxScore) {
				if (_pedestalController.Active)
					AddToScore(scoreAmount);
			} else if(!_readyFired) {
				if (MaxScoreReachedEvent != null) {
					MaxScoreReachedEvent(this);
				}

				SetDisplayText("Ready!");
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

	private void SetDisplayText(string text) {
		scoreText.text = text;
	}

	// event handlers
	void HandleAtPathEndEvent (ZMWaypointMovement lobbyPedestalController)
	{
		/*if (lobbyPedestalController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			_pedestalAtEnd = true;
			scoreBar.gameObject.SetActive(true);
		}*/
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (_playerInfo.playerTag.Equals(playerTag)) {
			gameObject.SetActive(true);
			light.enabled = true;
		}
	}
}
