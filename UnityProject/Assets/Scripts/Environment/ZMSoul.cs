﻿using UnityEngine;
using System.Collections;
using ZMPlayer;

public class ZMSoul : MonoBehaviour {
	private ZMScoreController _scoreController;

	private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	public delegate void SoulDestroyedAction(ZMSoul soul); public static event SoulDestroyedAction SoulDestroyedEvent;

	private bool _fadingIn;

	void Awake () {
		_playerInfo = GetComponent<ZMPlayerInfo>();

		ZMScoreController.MinScoreReached += HandleMinScoreReached;
		ZMScoreController.CanScoreEvent += HandleCanScoreEvent;
		ZMScoreController.StopScoreEvent += HandleStopScoreEvent;
	}

	void HandleStopScoreEvent (ZMScoreController scoreController)
	{
		if (audio.isPlaying) {
			StopLoop();
		}
	}

	void HandleCanScoreEvent (ZMScoreController scoreController)
	{
		if (!audio.isPlaying) {
			PlayLoop();
		}
	}

	void OnDestroy() {
		SoulDestroyedEvent = null;
	}

	void HandleMinScoreReached (ZMScoreController scoreController)
	{
		if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
			if (SoulDestroyedEvent != null) {
				SoulDestroyedEvent(this);
			}
		}
	}

	void Start() {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		
		foreach (GameObject player in players) {
			ZMScoreController scoreController = player.GetComponent<ZMScoreController>();
			
			if (scoreController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				_scoreController = scoreController;
			}
		}
	}

	void Update() {
		if (_fadingIn) {
			if (audio.volume < 0.75f)
				audio.volume += 0.02f;
		} else {
			if (audio.volume > 0)
				audio.volume -= 0.04f;
			else
				audio.Stop();
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
	}
}
