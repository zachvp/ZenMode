
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMPlayer;
using Core;

public class ZMPedestalController : ZMPlayerItem
{
	public ParticleSystem zenAbsorbEffect;
	public ParticleSystem zenPop;
	public float moveSpeed;
	
	private enum ScoreState { SCORING_ENABLED, SCORING_DISABLED };

	private const int RESPAWN_TIME = 5;
	private ScoreState _scoreState;

	// scaling
	private Vector3 _baseScale;
	private bool _shouldScale;

	// references
	private HashSet<ZMPlayerInfo> _scoringAgents;
	private List<ParticleSystem> _zenPopSystems;
	private Light _light;
	private ZMScalePulse _growShrink;
	private Renderer _renderer;

	private const string kPedestalWaypointTag = "PedestalWaypoint";
	private const string kDisableMethodName   = "Disable";

	// delegates
	public static EventHandler<ZMPedestalController> OnActivateEvent;
	public static EventHandler<ZMPedestalController> OnDeactivateEvent;

	protected override void Awake()
	{
		base.Awake();

		_light = GetComponent<Light>();
		_growShrink = GetComponent<ZMScalePulse>();
		_renderer = GetComponent<Renderer>();

		_scoringAgents = new HashSet<ZMPlayerInfo>();
		_zenPopSystems = new List<ParticleSystem>();
		_baseScale = transform.localScale;

		// event handler subscriptions
		ZMStageScoreController.CanScoreEvent 	     += HandleCanScoreEvent;
		ZMStageScoreController.OnStopScore   	     += HandleStopScoreEvent;
		ZMStageScoreController.OnReachMinScore	     += HandleMinScoreReached;

		Disable();
	}

	protected void Start()
	{
		_renderer.material.color = new Color(_playerInfo.standardColor.r,
											 _playerInfo.standardColor.g,
											 _playerInfo.standardColor.b,
											 0.5f);
	}

	void Update()
	{
		if (IsEnabled() && _shouldScale)
		{
			Vector3 newScale = Vector3.Lerp(transform.localScale, _baseScale, 5.0f * Time.deltaTime);

			transform.localScale = newScale;

			if (Vector3.SqrMagnitude(transform.localScale - _baseScale) < 0.7f)
			{
				_growShrink.Resume();
				_shouldScale = false;
			}
		}
	}

	void OnDestroy()
	{
		OnActivateEvent   = null;
		OnDeactivateEvent = null;
	}

	protected override void AcceptPlayerEvents()
	{
		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRespawnEvent += HandleSpawnObjectEvent;
	}

	// public methods
	public void Enable()
	{
		_scoreState = ScoreState.SCORING_ENABLED;
		_renderer.enabled = true;

		zenAbsorbEffect.Play();

		_light.enabled = true;

		Notifier.SendEventNotification(OnActivateEvent, this);
	}

	private void Disable()
	{
		_scoreState = ScoreState.SCORING_DISABLED;
		_renderer.enabled = false;
		zenAbsorbEffect.Stop();

		_light.enabled = false;

		Notifier.SendEventNotification(OnDeactivateEvent, this);
	}

	private void MoveToLocation(Vector3 location)
	{
		Vector3 newLocation = new Vector3(location.x, location.y, transform.position.z);

		gameObject.transform.position = newLocation;
	}

	public bool IsEnabled() { return _scoreState == ScoreState.SCORING_ENABLED; }
	public bool IsDiabled() { return _scoreState == ScoreState.SCORING_DISABLED; }

	// event handlers
	private void HandlePlayerDeathEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			var scoreController = ZMPlayerManager.Instance.Scores[info.ID];

			if (scoreController.TotalScore <= 0) { return; }

			_growShrink.Resume();

			MoveToLocation(scoreController.transform.position);
			Enable();

			if (IsInvoking(kDisableMethodName)) { CancelInvoke(kDisableMethodName); }
		}
	}

	
	private void HandleSpawnObjectEvent(ZMPlayerController playerController)
	{
		if (_playerInfo == playerController.PlayerInfo)
		{
			Invoke(kDisableMethodName, 0.01f);
		}
	}

	void HandleMinScoreReached(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			zenPop.GetComponent<Renderer>().material.color = _renderer.material.color;

			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);
			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);
			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);

			Disable();
			Invoke("StopThePop", 0.08f);
		}
	}

	void StopThePop()
	{
		foreach (ParticleSystem system in _zenPopSystems) { system.Stop(); }

		Invoke ("ClearThePop", 2);
	}

	void ClearThePop()
	{
		foreach (ParticleSystem system in _zenPopSystems) { Destroy(system); }

		_zenPopSystems.Clear();
	}

	void HandleCanScoreEvent(ZMPlayerInfo info)
	{
		_scoringAgents.Add(info);
	}

	void HandleStopScoreEvent(ZMPlayerInfo info)
	{
		_scoringAgents.Remove(info);
	}
}
