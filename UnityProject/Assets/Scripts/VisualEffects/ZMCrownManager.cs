using UnityEngine;
using ZMPlayer;

public class ZMCrownManager : MonoBehaviour {
	private bool _lobbyDominator;
	private int _dominatorIndex;

	// references
	private ZMGameStateController _gameStateController;
	private GameObject[] _crowns;

	private static int _leadingPlayerIndex; public static int LeadingPlayerIndex { get { return _leadingPlayerIndex; } }

	void Awake() {
		_crowns = new GameObject[ZMPlayerManager.PlayerCount];
	}
	
	void Start () {
		// get ref to GameStateController
		_gameStateController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ZMGameStateController>();

		// find, store, and deactivate all the crowns
		GameObject[] crownObjects = GameObject.FindGameObjectsWithTag("Crown");

		foreach (GameObject crown in crownObjects) {
			int crownIndex = (int) crown.GetComponent<ZMPlayerInfo>().playerTag;

			crown.gameObject.SetActive(false);

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
			_crowns[maxKillIndex].SetActive(true);
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
				_crowns[i].SetActive(false);
		}
		
		if (maxScoreController != null && !_lobbyDominator) {
			_crowns[(int) maxScoreController.PlayerInfo.playerTag].SetActive(true);
		} else if (_lobbyDominator && _dominatorIndex < _crowns.Length) {
			_crowns[_dominatorIndex].SetActive(true);
		}
	}
}
