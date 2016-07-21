using UnityEngine;
using UnityEngine.UI;
using Core;
using ZMPlayer;
using ZMConfiguration;

public class ZMCrownManager : MonoBehaviour
{
	private bool _lobbyDominator;
	private int _dominatorIndex;

	// references
	private GameObject[] _crowns;
	private ZMScoreController[] _scores;

	private static int _leadingPlayerIndex; public static int LeadingPlayerIndex { get { return _leadingPlayerIndex; } }

	void Awake()
	{
		_crowns = new GameObject[Settings.MatchPlayerCount.value];

		MatchStateManager.OnMatchEnd += HandleGameEndEvent;

		ZMPlayerManager.Instance.OnAllPlayersSpawned += AcceptPlayerEvents;
	}

	void Start()
	{
		_scores = ZMPlayerManager.Instance.Scores;
		enabled = false;

		// find and store all the crowns
		GameObject[] crownObjects = GameObject.FindGameObjectsWithTag("Crown");

		foreach (GameObject crown in crownObjects) {
			int crownIndex = crown.GetComponent<ZMPlayerInfo>().ID;

			if (crownIndex < Settings.MatchPlayerCount.value)
			{
				_crowns[crownIndex] = crown;
			}
		}
		
		// enable the leading killing player's crown
		int maxKills = 0;
		int maxKillIndex = -1;
		
		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i) {
			if (Settings.LobbyKillcount.value[i] > maxKills) {
				maxKills = Settings.LobbyKillcount.value[i];
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
	void Update()
	{
		float maxScoreCrown = 0;
		float checkEquality = _scores[0].TotalScore;
		bool scoresEqual = false;
		ZMScoreController maxScoreController = null;

		// see if the scores are equal
		for (int i = 1; i < Settings.MatchPlayerCount.value; ++i)
		{
			_leadingPlayerIndex = -1;

			if (_scores[i].TotalScore == checkEquality) { scoresEqual = true; }
		}

		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i)
		{
			var scoreController = _scores[i];

			if (scoreController.TotalScore > maxScoreCrown && !scoresEqual)
			{
				_leadingPlayerIndex = i;
				_lobbyDominator = false;
				maxScoreCrown = scoreController.TotalScore;
				maxScoreController = scoreController;
			}
			
			if (_crowns[i] != null && !_lobbyDominator) { _crowns[i].GetComponent<Text>().color = Color.white; }
		}
		
		if (maxScoreController != null && !_lobbyDominator)
		{
			_crowns[maxScoreController.PlayerInfo.ID].GetComponent<Text>().color = Color.yellow;
		}
		else if (_lobbyDominator && _dominatorIndex < _crowns.Length)
		{
			_crowns[_dominatorIndex].GetComponent<Text>().color = Color.yellow;
		}
	}

	private void AcceptPlayerEvents()
	{
		ZMPlayerController.OnPlayerDeath += HandlePlayerDeathEvent;
		ZMPlayerController.OnPlayerRespawn += HandlePlayerRespawnEvent;

		enabled = true;
	}
	
	private void HandleGameEndEvent()
	{		
		for (int i = 0; i < _crowns.Length; ++i)
		{
			if (_crowns[i] != null) { _crowns[i].SetActive(false); }
		}
	}
	
	private void HandlePlayerRespawnEvent(ZMPlayerControllerEventArgs args)
	{
		if (!MatchStateManager.IsEnd())
		{
			_crowns[args.controller.PlayerInfo.ID].SetActive(true);
		}
	}
	
	private void HandlePlayerDeathEvent(ZMPlayerInfoEventArgs args)
	{
		_crowns[args.info.ID].SetActive(false);
	}
}
