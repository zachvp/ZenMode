using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ZMPlayer;

public class ZMLobbyScoreController : ZMPlayerItem
{
	public float maxScore = 100.0f;
	public float scoreAmount = 0.5f;
	public Slider scoreBar;

	public delegate void MaxScoreReachedAction(ZMLobbyScoreController lobbyScoreController); public static event MaxScoreReachedAction MaxScoreReachedEvent;

	// private members
	private float _currentScore;
	private bool _pedestalActive;
	private bool _readyFired;
	private bool _targetAlive;

	// references
	private ZMLobbyPedestalController _pedestalController;
	private Vector3 _basePosition;

	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();

		_currentScore = 0;
		_pedestalActive = false;
		_readyFired = false;
		_targetAlive = true;

		gameObject.SetActive(false);
		light.enabled = false;
		scoreBar.gameObject.SetActive(false);

		ZMLobbyController.PlayerJoinedEvent += HandlePlayerJoinedEvent;
		ZMLobbyController.DropOutEvent += HandleDropOutEvent;
		ZMLobbyPedestalController.ActivateEvent += HandleActivateEvent;

		UpdateUI();
		AcceptPlayerEvents();
	}

	protected void Start()
	{
		_basePosition = new Vector3 (transform.position.x, transform.position.y, transform.position.z);

		foreach (GameObject pedestal in GameObject.FindGameObjectsWithTag("Pedestal"))
		{
			ZMLobbyPedestalController controller = pedestal.GetComponent<ZMLobbyPedestalController>();
			
			if (_playerInfo == controller.PlayerInfo) { _pedestalController = controller; }
		}

		scoreBar.handleRect = null;
	}

	void OnDestroy()
	{
		MaxScoreReachedEvent = null;
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		if (collider.gameObject.Equals(_pedestalController.gameObject))
		{
			if (_currentScore < maxScore)
			{
				if (_pedestalActive && _targetAlive)
					AddToScore(scoreAmount);
			}
			else if(!_readyFired)
			{
				if (MaxScoreReachedEvent != null)
				{
					MaxScoreReachedEvent(this);
				}

				_readyFired = true;
			}
		}
	}

	protected override void AcceptPlayerEvents()
	{
		_playerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		_playerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;
	}
	
	private void HandlePlayerRespawnEvent(ZMPlayerController playerController)
	{
		if(_playerInfo == playerController.PlayerInfo)
		{
			_targetAlive = true;
		}
	}
	
	private void HandlePlayerDeathEvent(ZMPlayerController playerController)
	{
		if(_playerInfo == playerController.PlayerInfo)
		{
			_targetAlive = false;
		}
	}
	
	private void HandleDropOutEvent(int playerIndex)
	{
		if (playerIndex == _playerInfo.ID)
		{
			transform.position = _basePosition;
			gameObject.SetActive(false);
			scoreBar.gameObject.SetActive(false);
		}
	}
	
	private void HandleActivateEvent(ZMLobbyPedestalController lobbyPedestalController)
	{
		if (_playerInfo == lobbyPedestalController.PlayerInfo)
		{
			_pedestalActive = true;
			scoreBar.gameObject.SetActive(true);
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

	void HandlePlayerJoinedEvent(int controlIndex)
	{
		if (_playerInfo.ID == controlIndex)
		{
			gameObject.SetActive(true);
			light.enabled = true;

			GetComponent<ZMPlayerController>().EnablePlayer();
		}
	}
}
