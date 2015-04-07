using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ZMLobbyScoreController : MonoBehaviour {
	public float maxScore = 100.0f;
	public float scoreAmount = 0.5f;
	public Text scoreText;
	public Slider scoreBar;

	public delegate void MaxScoreReachedAction(ZMLobbyScoreController lobbyScoreController);
	public static event MaxScoreReachedAction MaxScoreReachedEvent;

	// private members
	private float _currentScore;
	private bool _readyFired;
	private bool _pedestalAtEnd;

	// consntants
	private const string kScoreFormat = "0.0";

	// Use this for initialization
	void Awake () {
		_currentScore = 0;
		_readyFired = false;
		_pedestalAtEnd = false;

		ZMLobbyPedestalController.AtPathEndEvent += HandleAtPathEndEvent;

		UpdateUI();
	}

	void OnDestroy() {
		MaxScoreReachedEvent = null;
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D collider) {
		if (collider.CompareTag("Pedestal")) {
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
		_pedestalAtEnd = true;
	}
}
