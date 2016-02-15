using UnityEngine;
using System.Collections;
using ZMPlayer;
using Core;

public class ZMSoul : MonoBehaviour
{
	private ZMScoreController _scoreController;

	private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	public delegate void SoulDestroyedAction(ZMSoul soul); public static event SoulDestroyedAction SoulDestroyedEvent;

	private bool _fadingIn;
	private bool _playingSound;

	private ParticleSystem _particles;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMStageScoreController.MinScoreReached += HandleMinScoreReached;
		ZMStageScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMStageScoreController.OnStopScore += HandleStopScoreEvent;

		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
		MatchStateManager.OnMatchPause += HandlePauseGameEvent;
		MatchStateManager.OnMatchResume += HandleResumeGameEvent;

		_particles = GetComponentInChildren<ParticleSystem>();
	}
	
	void Start()
	{
		_particles.GetComponent<Renderer>().material.color = Utilities.GetRGB(_particles.GetComponent<Renderer>().material.color, _playerInfo.standardColor);

		_scoreController = ZMPlayerManager.Instance.Scores[_playerInfo.ID];
	}

	void OnDestroy()
	{
		SoulDestroyedEvent = null;
	}

	void Update()
	{
		if (_fadingIn) {
			if (GetComponent<AudioSource>().volume < 0.75f) { GetComponent<AudioSource>().volume += 0.02f; }
		} else {
			if (GetComponent<AudioSource>().volume > 0) { GetComponent<AudioSource>().volume -= 0.02f; }
			else { GetComponent<AudioSource>().Stop(); }
		}
	}

	private void HandleGameEndEvent ()
	{
		enabled = false;
		
		Invoke ("Deactivate", 0.2f);
	}
	
	private void HandleResumeGameEvent ()
	{
		if (_playingSound) {
			PlayLoop();
		}
	}
	
	private void HandlePauseGameEvent ()
	{
		StopLoop();
		
		if (GetComponent<AudioSource>().isPlaying) {
			_playingSound = true;
		} else {
			_playingSound = false;
		}
	}
	
	private void HandleStopScoreEvent (ZMScoreController scoreController)
	{
		StopLoop();
	}
	
	private void HandleCanScoreEvent (ZMScoreController scoreController)
	{
		PlayLoop();
	}
	
	private void HandleMinScoreReached (ZMScoreController scoreController)
	{
		if (_playerInfo == scoreController.PlayerInfo)
		{
			GetComponent<AudioSource>().Stop();
			
			if (SoulDestroyedEvent != null) {
				SoulDestroyedEvent(this);
			}
		}
	}

	public void AddZen(float amount) {
		_scoreController.AddToScore(amount);
	}

	public float GetZen() {
		return _scoreController.TotalScore;
	}

	public void SetZen(float amount) {
		_scoreController.SetScore(amount);
	}

	private void PlayLoop() {
		_fadingIn = true;

		GetComponent<AudioSource>().volume = 0;
		GetComponent<AudioSource>().Play();
	}

	private void StopLoop() {
		_fadingIn = false;
		_playingSound = false;
	}

	void Deactivate() {
		gameObject.SetActive(false);
	}
}
