using UnityEngine;
using ZMPlayer;
using UnityEngine.UI;

public class ZMCrownManager : MonoBehaviour {
	private bool _lobbyDominator;
	private int _dominatorIndex;

	// references
	private ZMGameStateController _gameStateController;
	private GameObject[] _crowns;

	private static int _leadingPlayerIndex; public static int LeadingPlayerIndex { get { return _leadingPlayerIndex; } }

	private bool _endGame;

	void Awake() {
		_crowns = new GameObject[ZMPlayerManager.PlayerCount];

		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;
		ZMGameStateController.GameEndEvent += HandleGameEndEvent;
	}

	void HandleGameEndEvent ()
	{
		_endGame = true;

		for (int i = 0; i < _crowns.Length; ++i) {
			if (_crowns[i] != null) { _crowns[i].SetActive(false); }
		}
	}

	void HandlePlayerRespawnEvent (ZMPlayerController playerController)
	{
		if (!_endGame) {
			int index = (int) playerController.PlayerInfo.playerTag;
			
			_crowns[index].SetActive(true);
		}
	}

	void HandlePlayerDeathEvent (ZMPlayerController playerController)
	{
		int index = (int) playerController.PlayerInfo.playerTag;

		_crowns[index].SetActive(false);
	}
	
	void Start () {
		// get ref to GameStateController
		_gameStateController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZMGameStateController>();

		// find and store all the crowns
		GameObject[] crownObjects = GameObject.FindGameObjectsWithTag("Crown");

		foreach (GameObject crown in crownObjects) {
			int crownIndex = (int) crown.GetComponent<ZMPlayerInfo>().playerTag;
			if (crownIndex < ZMPlayerManager.PlayerCount) {
				_crowns[crownIndex] = crown;
			}
		}
		
		// enable the leading killing player's crown
		int maxKills = 0;
		int maxKillIndex = -1;
		
		for (int i = 0; i < ZMPlayerManager.PlayerCount; ++i) {
			if (ZMPlayerManager.PlayerKills[i] > maxKills) {
				maxKills = ZMPlayerManager.PlayerKills[i];
				maxKillIndex = i;
			}
		}
		
		if (maxKillIndex > -1 && _crowns[maxKillIndex] != null) {
			_lobbyDominator = true;
			_dominatorIndex = maxKillIndex;
			_crowns[maxKillIndex].GetComponent<Text>().color = Color.yellow;
		}
	}
	
	// Update is called once per frame
	void Update () {
		float maxScoreCrown = 0;
		float checkEquality = _gameStateController.ScoreControllers[0].TotalScore;
		bool scoresEqual = false;
		ZMScoreController maxScoreController = null;

		// see if the scores are equal
		for (int i = 1; i < ZMPlayerManager.PlayerCount; ++i) {
			_leadingPlayerIndex = -1;
			if (_gameStateController.ScoreControllers[i].TotalScore == checkEquality) {
				scoresEqual = true;
			}
		}

		for (int i = 0; i < ZMPlayerManager.PlayerCount; ++i) {
			ZMScoreController scoreController = _gameStateController.ScoreControllers[i];

			if (scoreController.TotalScore > maxScoreCrown && !scoresEqual) {
				_leadingPlayerIndex = i;
				_lobbyDominator = false;
				maxScoreCrown = scoreController.TotalScore;
				maxScoreController = scoreController;
			}
			
			if (_crowns[i] != null && !_lobbyDominator)
				_crowns[i].GetComponent<Text>().color = Color.white;
		}
		
		if (maxScoreController != null && !_lobbyDominator) {
			_crowns[(int) maxScoreController.PlayerInfo.playerTag].GetComponent<Text>().color = Color.yellow;
		} else if (_lobbyDominator && _dominatorIndex < _crowns.Length) {
			_crowns[_dominatorIndex].GetComponent<Text>().color = Color.yellow;
		}
	}
}
