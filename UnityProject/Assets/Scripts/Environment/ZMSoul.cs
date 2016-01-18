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
		_particles.renderer.material.color = Utilities.GetRGB(_particles.renderer.material.color, _playerInfo.standardColor);

		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		foreach (GameObject player in players) {
			ZMScoreController scoreController = player.GetComponent<ZMScoreController>();
			
			if (_playerInfo == scoreController.PlayerInfo)
			{
				_scoreController = scoreController;
			}
		}
	}

	void OnDestroy()
	{
		SoulDestroyedEvent = null;
	}

	void Update()
	{
		if (_fadingIn) {
			if (audio.volume < 0.75f) { audio.volume += 0.02f; }
		} else {
			if (audio.volume > 0) { audio.volume -= 0.02f; }
			else { audio.Stop(); }
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
		
		if (audio.isPlaying) {
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
			audio.Stop();
			
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

		audio.volume = 0;
		audio.Play();
	}

	private void StopLoop() {
		_fadingIn = false;
		_playingSound = false;
	}

	void Deactivate() {
		gameObject.SetActive(false);
	}
}
