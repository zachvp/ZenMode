using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMPlayer;

public class ZMPedestalController : MonoBehaviour {
	public ParticleSystem zenAbsorbEffect;
	public ParticleSystem zenPop;
	public TextMesh timerText;
	public float moveSpeed;

	private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	private enum ScoreState { SCORING_ENABLED, SCORING_DISABLED };
	private int RESPAWN_TIME = 5;
	private int currentTimer;
	private float lingerAfterSpawnTime = 0.0f;
	private ScoreState _scoreState;

	// scaling
	private Vector3 _baseScale;
	private bool _shouldScale;

	// references
	private HashSet<ZMScoreController> _scoringAgents;
	private List<ParticleSystem> _zenPopSystems;

	private const string kPedestalWaypointTag = "PedestalWaypoint";
	private const string kDisableMethodName   = "Disable";

	// delegates
	public delegate void ActivateAction(ZMPedestalController pedestalController); public static event ActivateAction ActivateEvent;
	public delegate void DeactivateAction(ZMPedestalController pedestalController); public static event DeactivateAction DeactivateEvent;

	void Awake() {
		_scoringAgents = new HashSet<ZMScoreController>();
		_zenPopSystems = new List<ParticleSystem>();
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_baseScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

		//_moveState = MoveState.MOVE;
		// event handler subscriptions
		ZMPlayerController.PlayerDeathEvent      += HandlePlayerDeathEvent;
		ZMScoreController.UpdateScoreEvent       += HandleUpdateScoreEvent;
		ZMScoreController.CanScoreEvent 	     += HandleCanScoreEvent;
		ZMScoreController.StopScoreEvent   	     += HandleStopScoreEvent;
		ZMScoreController.MinScoreReached	     += HandleMinScoreReached;
		ZMGameStateController.SpawnObjectEvent   += HandleSpawnObjectEvent;

		Disable();
	}

	void Start () {
		currentTimer = RESPAWN_TIME;
		timerText.text = currentTimer.ToString ();
	}

	void FixedUpdate() {
		if (IsEnabled() && _shouldScale) {
			Vector3 newScale = Vector3.Lerp(transform.localScale, _baseScale, 5.0f * Time.deltaTime);

			transform.localScale = newScale;

			if (Vector3.SqrMagnitude(transform.localScale - _baseScale) < 0.7f) {
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

	private void ToggleOn() {
		Invoke("ToggleEnabled", 2.0f);
	}

	private void ToggleOff() {
		Invoke("ToggleEnabled", 2.0f);
	}

	// private methods
	private void ToggleEnabled() {
		if (_scoreState == ScoreState.SCORING_ENABLED) {
			Disable();
			ToggleOn();
		} else if (_scoreState == ScoreState.SCORING_DISABLED) {
			Enable();
			ToggleOff();
		}
	}

	private void CountdownText() {
		if (currentTimer > 0) {
			currentTimer--;
			timerText.text = currentTimer.ToString ();
			Invoke ("CountdownText", 1.0f);
		}
	}

	// public methods
	public void Enable() {
		_scoreState = ScoreState.SCORING_ENABLED;
		renderer.enabled = true;
		zenAbsorbEffect.Play();
		if (timerText.renderer.enabled == false) {
			currentTimer = RESPAWN_TIME;
			timerText.text = RESPAWN_TIME.ToString();
			timerText.renderer.enabled = true;
			Invoke ("CountdownText", 1.0f);
		}

		if (light != null)
			light.enabled = true;

		// notify event handlers
		if (ActivateEvent != null) {
			ActivateEvent(this);
		}
	}

	private void Disable() {
		_scoreState = ScoreState.SCORING_DISABLED;
		renderer.enabled = false;
		zenAbsorbEffect.Stop();
		timerText.renderer.enabled = false;

		if (light != null) {
			light.enabled = false;
		}

		if (DeactivateEvent != null) {
			DeactivateEvent(this);
		}
	}

	private void MoveToLocation(Vector3 location) {
		Vector3 newLocation = new Vector3(location.x, location.y, transform.position.z);

		gameObject.transform.position = newLocation;
	}

	public bool IsEnabled() { return _scoreState == ScoreState.SCORING_ENABLED; }
	public bool IsDiabled() { return _scoreState == ScoreState.SCORING_DISABLED; }

	// event handlers
	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			ZMScoreController scoreController = playerController.GetComponent<ZMScoreController>();

			if (scoreController.TotalScore <= 0) { return; }

			SendMessage("Stop");
			transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
			_shouldScale = true;

			MoveToLocation(playerController.transform.position);
			Enable();

			if (IsInvoking(kDisableMethodName)) {
				CancelInvoke(kDisableMethodName);
			}
		}
	}

	void HandleMinScoreReached (ZMScoreController scoreController)
	{
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			//ZMScoreController.MinScoreReached -= HandleMinScoreReached;

			zenPop.renderer.material.color = renderer.material.color;

			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);
			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);
			_zenPopSystems.Add(ParticleSystem.Instantiate(zenPop, transform.position, transform.rotation) as ParticleSystem);

			Disable();
			Invoke ("StopThePop", 0.08f);
			//gameObject.SetActive(false);
		}
	}

	void StopThePop() {
		foreach (ParticleSystem system in _zenPopSystems) {
			system.Stop();
		}

		Invoke ("ClearThePop", 2);
	}

	void ClearThePop() {
		foreach (ParticleSystem system in _zenPopSystems) {
			Destroy(system);
		}

		_zenPopSystems.Clear();
	}

	void HandleUpdateScoreEvent(ZMScoreController scoreController) {
		float scoreSum = 0;

		foreach (ZMScoreController agent in _scoringAgents) {
			scoreSum += agent.TotalScore;
		}
	}

	void HandleCanScoreEvent(ZMScoreController scoreController) {
		if (!_scoringAgents.Contains(scoreController)) {
			_scoringAgents.Add(scoreController);
		}
	}

	void HandleStopScoreEvent (ZMScoreController scoreController) {
		_scoringAgents.Remove(scoreController);
	}

	void HandleSpawnObjectEvent(ZMGameStateController gameStateController, ZMPlayerController playerController) {
		if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			Invoke(kDisableMethodName, lingerAfterSpawnTime);
		}
	}
}
