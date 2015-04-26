using UnityEngine;
using System.Collections;

public class ZMPlayerManager : MonoBehaviour {
	// the number of players that will be in the current game
	private int _numPlayers; 
	public int NumPlayers { get { return _numPlayers; } }
	
	void Awake () {
		_numPlayers = 3;
		DontDestroyOnLoad(gameObject);

		ZMLobbyController.PlayerReadyEvent += HandlePlayerReadyEvent;;
		ZMGameStateController.GameEndEvent += HandleGameEndEvent;
	}

	void HandleGameEndEvent ()
	{
		Destroy(gameObject);
	}

	void HandlePlayerReadyEvent (ZMPlayer.ZMPlayerInfo.PlayerTag playerTag)
	{
		_numPlayers += 1;
	}
}
