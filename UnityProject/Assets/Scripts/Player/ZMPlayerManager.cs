using UnityEngine;
using ZMPlayer;

public class ZMPlayerManager : MonoBehaviour {
	public bool debug = false;
	public int debugPlayerCount = 2;

	public const int MAX_PLAYERS = 4;

	private enum State { NONE, LOBBY, STAGE }; private static State _state;
	private static int _playerCount; public static int PlayerCount { get { return _playerCount; } }
	private static GameObject[] _players; public static GameObject[] Players { get { return _players; } }

	private bool[] _readiedPlayers;

	// stats
	private static int[] _playerKills; public static int[] PlayerKills { get { return _playerKills; } }
	
	void Awake () {
		if (_readiedPlayers == null) { _readiedPlayers = new bool[MAX_PLAYERS]; }
		if (_playerKills == null) { _playerKills = new int[MAX_PLAYERS]; }
		if (_players == null) { _players = new GameObject[MAX_PLAYERS]; }

		switch(_state) {
			case State.NONE:  {
				_state = State.LOBBY;

				_playerCount = 0;
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

		if (debug) {
			_state = State.STAGE;
			_playerCount = debugPlayerCount;
		}

		DontDestroyOnLoad(gameObject);
	}

	void Start() {
		GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");

		for (int i = 0; i < MAX_PLAYERS; ++i) {
			ZMPlayerInfo playerInfo = playerObjects[i].GetComponent<ZMPlayerInfo>();

			_players[(int) playerInfo.playerTag] = playerObjects[i];
		}
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
		_playerCount += 1;
	}
}
