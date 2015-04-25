﻿using UnityEngine;
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
	private bool _pedestalAtEnd;
	private ZMPlayer.ZMPlayerInfo _playerInfo;

	// consntants
	private const string kScoreFormat = "0.0";

	// Use this for initialization
	void Awake () {
		_currentScore = 0;
		_readyFired = false;
		_pedestalAtEnd = false;
		_playerInfo = GetComponent<ZMPlayer.ZMPlayerInfo>();
		gameObject.SetActive(false);
		light.enabled = false;

		ZMLobbyPedestalController.AtPathEndEvent += HandleAtPathEndEvent;
		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;

		UpdateUI();
	}

	void OnDestroy() {
		MaxScoreReachedEvent = null;
	}

	void OnTriggerStay2D(Collider2D collider) {
		if (collider.CompareTag("Pedestal")) {
			if (collider.GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag.Equals(_playerInfo.playerTag)) {
				Debug.Log(gameObject.name + ": my player tag");
				if (_currentScore < maxScore) {

					if (_pedestalAtEnd)
						AddToScore(scoreAmount);
				} else if(!_readyFired) {
					if (MaxScoreReachedEvent != null) {
						MaxScoreReachedEvent(this);
					}

					UpdateText("Ready!");
					_readyFired = true;
				}
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
		
		scoreText.text = normalizedScore.ToString(kScoreFormat) + "%";
		
		scoreBar.value = normalizedScore; 
	}

	private void UpdateText(string text) {
		scoreText.text = text;
	}

	// event handlers
	void HandleAtPathEndEvent (ZMLobbyPedestalController lobbyPedestalController)
	{
		Debug.Log(gameObject.name + ": orb at path end " + lobbyPedestalController.PlayerInfo.playerTag.ToString());
		if (lobbyPedestalController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			_pedestalAtEnd = true;
		}
	}

	void HandlePlayerJoinedEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		if (_playerInfo.playerTag.Equals(playerTag)) {
			gameObject.SetActive(true);
			light.enabled = true;
		}
	}
}
