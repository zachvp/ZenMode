using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMPlayer;

public class ZMPedestalController : ZMPlayerItem
{
	public ParticleSystem zenAbsorbEffect;
	public ParticleSystem zenPop;
	public TextMesh timerText;
	public float moveSpeed;
	
	private enum ScoreState { SCORING_ENABLED, SCORING_DISABLED };
	private int RESPAWN_TIME = 5;
	private int currentTimer;
	private float lingerAfterSpawnTime = 0.0f;
	private ScoreState _scoreState;

	// scaling
	private Vector3 _baseScale;
	private bool _shouldScale;

	// references
	private HashSet<ZMPlayerInfo> _scoringAgents;
	private List<ParticleSystem> _zenPopSystems;

	private const string kPedestalWaypointTag = "PedestalWaypoint";
	private const string kDisableMethodName   = "Disable";

	// delegates
	public delegate void ActivateAction(ZMPedestalController pedestalController); public static event ActivateAction ActivateEvent;
	public delegate void DeactivateAction(ZMPedestalController pedestalController); public static event DeactivateAction DeactivateEvent;

	protected override void Awake()
	{
		base.Awake();

		_scoringAgents = new HashSet<ZMPlayerInfo>();
		_zenPopSystems = new List<ParticleSystem>();
		_baseScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

		// event handler subscriptions
		ZMStageScoreController.CanScoreEvent 	     += HandleCanScoreEvent;
		ZMStageScoreController.OnStopScore   	     += HandleStopScoreEvent;
		ZMStageScoreController.OnReachMinScore	     += HandleMinScoreReached;

		Disable();
	}

	protected void Start()
	{
		currentTimer = RESPAWN_TIME;

		timerText.text = currentTimer.ToString();
	}

	void FixedUpdate()
	{
		if (IsEnabled() && _shouldScale)
		{
			Vector3 newScale = Vector3.Lerp(transform.localScale, _baseScale, 5.0f * Time.deltaTime);

			transform.localScale = newScale;

			if (Vector3.SqrMagnitude(transform.localScale - _baseScale) < 0.7f)
			{
				SendMessage("Resume");
				_shouldScale = false;
			}
		}
	}

	void OnDestroy() {
		// unsubscribe all event listeners
		ActivateEvent  	= null;
		DeactivateEvent = null;
	}

	protected override void AcceptPlayerEvents()
	{
		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRespawnEvent += HandleSpawnObjectEvent;
	}
	
	private void CountdownText()
	{
		if (currentTimer > 0) {
			currentTimer--;
			timerText.text = currentTimer.ToString();
			Invoke ("CountdownText", 1.0f);
		}
	}

	// public methods
	public void Enable()
	{
		_scoreState = ScoreState.SCORING_ENABLED;
		GetComponent<Renderer>().enabled = true;

		timerText.GetComponent<Renderer>().material.color = Color.white;
		zenAbsorbEffect.Play();

		if (timerText.GetComponent<Renderer>().enabled == false) {
			currentTimer = RESPAWN_TIME;
			timerText.text = RESPAWN_TIME.ToString();
			timerText.GetComponent<Renderer>().enabled = true;
			Invoke ("CountdownText", 1.0f);
		}

		if (GetComponent<Light>() != null)
			GetComponent<Light>().enabled = true;

		// notify event handlers
		if (ActivateEvent != null) {
			ActivateEvent(this);
		}
	}

	private void Disable()
	{
		_scoreState = ScoreState.SCORING_DISABLED;
		GetComponent<Renderer>().enabled = false;
		zenAbsorbEffect.Stop();
		timerText.GetComponent<Renderer>().enabled = false;

		if (GetComponent<Light>() != null) {
			GetComponent<Light>().enabled = false;
		}

		if (DeactivateEvent != null) {
			DeactivateEvent(this);
		}
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

			SendMessage("Stop");
			transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			_shouldScale = true;

			MoveToLocation(scoreController.transform.position);
			Enable();

			if (IsInvoking(kDisableMethodName)) { CancelInvoke(kDisableMethodName); }
		}
	}

	
	private void HandleSpawnObjectEvent(ZMPlayerController playerController)
	{
		if (_playerInfo == playerController.PlayerInfo)
		{
			Invoke(kDisableMethodName, lingerAfterSpawnTime);
		}
	}

	void HandleMinScoreReached(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			zenPop.GetComponent<Renderer>().sharedMaterial.color = GetComponent<Renderer>().material.color;

			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);
			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);
			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);

			Disable();
			Invoke("StopThePop", 0.08f);
		}
	}

	void StopThePop()
	{
		foreach (ParticleSystem system in _zenPopSystems) {
			system.Stop();
		}

		Invoke ("ClearThePop", 2);
	}

	void ClearThePop()
	{
		foreach (ParticleSystem system in _zenPopSystems)
		{
			Destroy(system);
		}

		_zenPopSystems.Clear();
	}

	void HandleCanScoreEvent(ZMPlayerInfo info)
	{
		if (!_scoringAgents.Contains(info))
		{
			_scoringAgents.Add(info);
		}
	}

	void HandleStopScoreEvent(ZMPlayerInfo info)
	{
		_scoringAgents.Remove(info);
	}
}
