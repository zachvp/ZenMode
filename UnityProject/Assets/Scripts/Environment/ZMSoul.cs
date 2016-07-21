using UnityEngine;
using System.Collections;
using ZMPlayer;
using Core;

public class ZMSoul : MonoBehaviour
{
	public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	private ZMScoreController _scoreController;

	private ZMPlayerInfo _playerInfo;

	public static EventHandler<MonoBehaviourEventArgs> SoulDestroyedEvent;

	private bool _fadingIn;
	private bool _playingSound;

	private ParticleSystem _particles;
	private AudioSource _audio;

	void Awake()
	{
		ZMStageScoreController.OnReachMinScore += HandleMinScoreReached;
		ZMStageScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMStageScoreController.OnStopScore += HandleStopScoreEvent;

		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
		MatchStateManager.OnMatchPause += HandlePauseGameEvent;
		MatchStateManager.OnMatchResume += HandleResumeGameEvent;

		_playerInfo = GetComponent<ZMPlayerInfo>();
		_particles = GetComponentInChildren<ParticleSystem>();
		_audio = GetComponent<AudioSource>();
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
		if (_fadingIn)
		{
			if (_audio.volume < 0.75f) { _audio.volume += 0.02f; }
		}
		else
		{
			if (_audio.volume > 0) { _audio.volume -= 0.02f; }
			else { _audio.Stop(); }
		}
	}

	private void HandleGameEndEvent()
	{
		enabled = false;
		
		Invoke ("Deactivate", 0.2f);
	}
	
	private void HandleResumeGameEvent()
	{
		if (_playingSound)
		{
			PlayLoop();
		}
	}
	
	private void HandlePauseGameEvent()
	{
		StopLoop();
		
		if (_audio.isPlaying) {
			_playingSound = true;
		} else {
			_playingSound = false;
		}
	}
	
	private void HandleStopScoreEvent(ZMPlayerInfoEventArgs args)
	{
		StopLoop();
	}
	
	private void HandleCanScoreEvent(ZMPlayerInfoEventArgs args)
	{
		PlayLoop();
	}
	
	private void HandleMinScoreReached(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			var outArgs = new MonoBehaviourEventArgs(this);

			_audio.Stop();

			Notifier.SendEventNotification(SoulDestroyedEvent, outArgs);
		}
	}

	public void AddZen(float amount)
	{
		_scoreController.AddToScore(amount);
	}

	public float GetZen()
	{
		return _scoreController.TotalScore;
	}

	public void SetZen(float amount)
	{
		_scoreController.SetScore(amount);
	}

	private void PlayLoop()
	{
		_fadingIn = true;

		_audio.volume = 0;
		_audio.Play();
	}

	private void StopLoop()
	{
		_fadingIn = false;
		_playingSound = false;
	}

	void Deactivate()
	{
		gameObject.SetActive(false);
	}
}
