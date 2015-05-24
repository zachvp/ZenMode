using UnityEngine;
using System.Collections.Generic;
using ZMPlayer;

public class ZMLobbyController : MonoBehaviour {
	public GameObject loadScreen;

	public delegate void PlayerJoinedAction(ZMPlayerInfo.PlayerTag playerTag); public static event PlayerJoinedAction PlayerJoinedEvent;
	public delegate void PlayerReadyAction(ZMPlayerInfo.PlayerTag playerTag); public static event PlayerReadyAction PlayerReadyEvent;
	public delegate void PauseGameAction(int playerIndex); public static event PauseGameAction PauseGameEvent;
	public delegate void ResumeGameAction(); public static event ResumeGameAction ResumeGameEvent;
	public delegate void DropOutAction(int playerIndex); public static event DropOutAction DropOutEvent;
	
	private int _requiredPlayerCount;
	private int _currentJoinCount; // i.e. how many  have pressed a button to join
	private int _currentReadyCount; // i.e. how many have actually readied up

	private bool _paused;
	int _playerPauseIndex;

	private bool[] _joinedPlayers;
	private bool[] _readyPlayers;

	// pause menu options
	private const int RESUME_OPTION   = 0;
//	private const int DROP_OUT_OPTION = 1
	private const int QUIT_OPTION 	  = 1;

	void Awake() {
		_currentJoinCount = 0;
		_currentReadyCount = 0;
		_paused = false;
		_joinedPlayers = new bool[ZMPlayerManager.MAX_PLAYERS];
		_readyPlayers = new bool[ZMPlayerManager.MAX_PLAYERS];

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;

		ZMGameInputManager.StartInputEvent		+= HandleStartInputEvent;
		ZMGameInputManager.AnyInputEvent 		+= HandleMainInputEvent;
		ZMMenuOptionController.SelectOptionEvent += HandleSelectOptionEvent;
	}

	void Start() {
		loadScreen.SetActive(false);
	}

	void OnDestroy() {
		PlayerJoinedEvent = null;
		PauseGameEvent    = null;
		PlayerReadyEvent  = null;
		ResumeGameEvent	  = null;
		DropOutEvent	  = null;
	}

	void HandleSelectOptionEvent(int optionIndex) {
		Time.timeScale = 1;

		switch(optionIndex) {
			case RESUME_OPTION: {
				HandleSelectResumeEvent();
				break;
			}
			/*case DROP_OUT_OPTION: {
				HandleSelectDropOutEvent();
				break;
			}*/
			case QUIT_OPTION: {
				HandleSelectQuitEvent();
				break;
			}
			default: break;
		}
	}

	void HandleSelectQuitEvent ()
	{
		Application.LoadLevel(ZMSceneIndexList.INDEX_MAIN_MENU);
	}

	void HandleMainInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		int playerIndex = (int) playerTag;

		if (!_joinedPlayers[playerIndex]) {
			_currentJoinCount += 1;
			
			if (PlayerJoinedEvent != null) {
				PlayerJoinedEvent(playerTag);
			}

			_requiredPlayerCount += 1;
			_joinedPlayers[playerIndex] = true;
		}
	}

	void HandleSelectDropOutEvent() {
		_joinedPlayers[_playerPauseIndex] = false;
		_currentJoinCount -= 1;

		if (_readyPlayers[_playerPauseIndex]) {
			_readyPlayers[_playerPauseIndex] = false;
			_currentReadyCount -= 1;
		}

		if (DropOutEvent != null) {
			DropOutEvent(_playerPauseIndex);
		}
	}

	void HandleSelectResumeEvent ()
	{
		_paused = false;

		if (ResumeGameEvent != null) {
			ResumeGameEvent();
		}
	}

	void HandleStartInputEvent (ZMPlayerInfo.PlayerTag playerTag)
	{
		if (_joinedPlayers[(int) playerTag]) {
			if (!_paused) {
				Time.timeScale = 0;

				_playerPauseIndex = (int) playerTag;
				_paused = true;

				if (PauseGameEvent != null) {
					PauseGameEvent(_playerPauseIndex);
				}
			}
		}
	}

	private void HandleMaxScoreReachedEvent(ZMLobbyScoreController scoreController) {
		_currentReadyCount += 1;
		_readyPlayers[(int) scoreController.PlayerInfo.playerTag] = true;

		if (PlayerReadyEvent != null) {
			PlayerReadyEvent(scoreController.PlayerInfo.playerTag);
		}

		if(_currentReadyCount > 1 && _currentReadyCount == _requiredPlayerCount) {
			Invoke("LoadLevel", 1.0f);
			Invoke("ShowLoadScreen", 0.5f);
		}
	}

	void ShowLoadScreen() {
		loadScreen.SetActive(true);
	}

	void LoadLevel() {
		Application.LoadLevel(ZMSceneIndexList.INDEX_STAGE);
	}
}
