using UnityEngine;
using System.Collections;

public class ZMPlayerManager : MonoBehaviour {
	public bool debug = false;
	public int debugPlayerCount = 2;

	private enum State { NONE, MENU, LOBBY, STAGE };

	private static State _state;
	private static int _numPlayers; 
	public static int NumPlayers { get { return _numPlayers; } }
	
	void Awake () {
		if (debug) {
			_state = State.STAGE;
			_numPlayers = debugPlayerCount;
		}

		switch(_state) {
			case State.NONE:  {
				_state = State.MENU;
				_numPlayers = 0;
				break;
			}
			case State.MENU:  {
				_state = State.LOBBY;
				break;
			}
			case State.LOBBY: {
				_state = State.STAGE;
				break;
			}
			default: {
				break;
			}
		}


		DontDestroyOnLoad(gameObject);

		ZMLobbyController.PlayerReadyEvent += HandlePlayerReadyEvent;
		ZMPauseMenuController.SelectQuitEvent += HandleSelectQuitEvent;
	}

	void HandleSelectQuitEvent() {
		_state = State.NONE;
		_numPlayers = 0;
	}

	void HandlePlayerReadyEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		_numPlayers += 1;
	}
}
