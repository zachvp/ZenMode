using UnityEngine;
using UnityEngine.UI;
using ZMPlayer;
using Core;
using ZMConfiguration;

public class ZMLobbyScoreController : ZMScoreController
{
	[SerializeField] private float maxScore = 100.0f;
	[SerializeField] private float scoreAmount = 0.5f;

	// private members
	private bool _pedestalActive;
	private bool _readyFired;
	private bool _targetAlive;

	// references
	private Vector3 _basePosition;

	// Use this for initialization
	protected override void Awake()
	{
		base.Awake();

		_targetAlive = true;
		_basePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);

		ZMLobbyController.OnPlayerDropOut += HandleDropOutEvent;
		ZMLobbyPedestalController.ActivateEvent += HandleActivateEvent;

		AcceptPlayerEvents();
	}

	void OnTriggerStay2D(Collider2D collider)
	{
		// Bail if the collider isn't an orb.
		if (!collider.CompareTag(Tags.kPedestal)) { return; }

		var info = collider.GetComponent<ZMPlayerInfo>();

		if (_playerInfo == info)
		{
			Debug.LogFormat("{0} overlapping {1}", name, info.name);

			if (_totalScore < maxScore)
			{
				if (_pedestalActive && _targetAlive) { AddToScore(scoreAmount); }
			}
			else if (!_readyFired)
			{
				Notifier.SendEventNotification(OnReachMaxScore, _playerInfo);

				_readyFired = true;
			}
		}
	}

	protected override void InitScore()
	{
		SetScore(0.0f);
	}

	protected override void AcceptPlayerEvents()
	{
		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;
	}
	
	private void HandlePlayerRespawnEvent(ZMPlayerController playerController)
	{
		if (_playerInfo == playerController.PlayerInfo)
		{
			_targetAlive = true;
		}
	}
	
	private void HandlePlayerDeathEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			_targetAlive = false;
		}
	}
	
	private void HandleDropOutEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			transform.position = _basePosition;
			gameObject.SetActive(false);
		}
	}
	
	private void HandleActivateEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info) { _pedestalActive = true; }
	}	
}
