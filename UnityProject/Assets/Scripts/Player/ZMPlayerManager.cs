﻿using UnityEngine;
using System.Collections;

public class ZMPlayerManager : MonoBehaviour {
	public bool debug = false;
	public int debugPlayerCount = 2;

	private enum State { NONE, LOBBY, STAGE };

	private static State _state;
	private static int _numPlayers; public static int NumPlayers { get { return _numPlayers; } }
	private bool[] _readiedPlayers;

	// stats
	private static int[] _playerKills; public static int[] PlayerKills { get { return _playerKills; } }
	
	void Awake () {
		if (debug) {
			_state = State.STAGE;
			_numPlayers = debugPlayerCount;
		}

		if (_readiedPlayers == null) {
			_readiedPlayers = new bool[4];
		}

		if (_playerKills == null) {
			_playerKills = new int[4];
		}

		switch(_state) {
			case State.NONE:  {
				_state = State.LOBBY;

				_numPlayers = 0;
				ZMLobbyController.PlayerReadyEvent += HandlePlayerReadyEvent;
				ZMPlayerController.PlayerKillEvent += HandlePlayerKillEvent;
				break;
			}
			case State.LOBBY: {
				_state = State.STAGE;

				ZMGameStateController.QuitMatchEvent += HandleSelectQuitEvent;
				ZMGameStateController.GameEndEvent   += HandleGameEndEvent;

				break;
			}
			default: {
				break;
			}
		}


		DontDestroyOnLoad(gameObject);
	}

	void HandlePlayerKillEvent (ZMPlayerController killer)
	{
		int killerIndex = (int) killer.PlayerInfo.playerTag;

		_playerKills[killerIndex] += 1;
	}

	void HandleGameEndEvent ()
	{
		Destroy(gameObject);
	}

	void HandleSelectQuitEvent() {
		_state = State.NONE;

		// unsubscribe events on quit since this object isn't destroyed
		ZMLobbyController.PlayerReadyEvent   -= HandlePlayerReadyEvent;
		ZMGameStateController.QuitMatchEvent -= HandleSelectQuitEvent;
	}

	void HandlePlayerReadyEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag) {
		_numPlayers += 1;
	}
}
