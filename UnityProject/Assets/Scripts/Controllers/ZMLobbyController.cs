using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ZMPlayer;
using Core;
using ZMConfiguration;

public class ZMLobbyController : MonoBehaviour {
	public GameObject loadScreen;
	public Text message;

	public static EventHandler<int> PlayerJoinedEvent;

	public static EventHandler<ZMPlayerInfo> PlayerReadyEvent;
	public static EventHandler<int> DropOutEvent;
	public static EventHandler<int> PauseGameEvent;
	public static EventHandler ResumeGameEvent;
	
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
		_joinedPlayers = new bool[Constants.MAX_PLAYERS];
		_readyPlayers = new bool[Constants.MAX_PLAYERS];

		message.text = "";

		ZMLobbyScoreController.MaxScoreReachedEvent += HandleMaxScoreReachedEvent;

		ZMGameInputManager.StartInputEvent		 += HandleStartInputEvent;
		ZMGameInputManager.AnyInputEvent 		 += HandleMainInputEvent;

		ZMPauseMenu.SelectOptionEvent += HandleSelectOptionEvent;
	}

	void Start()
	{
		loadScreen.SetActive(false);
	}

	void OnDestroy()
	{
		PlayerJoinedEvent = null;
		PauseGameEvent    = null;
		PlayerReadyEvent  = null;
		ResumeGameEvent	  = null;
		DropOutEvent	  = null;
	}

	void HandleSelectOptionEvent(int optionIndex)
	{
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

	void HandleMainInputEvent (int controlIndex)
	{
		if (!_joinedPlayers[controlIndex])
		{
			if (controlIndex > 0 && !_joinedPlayers[controlIndex - 1])
			{
				message.text = "Player " + controlIndex + " must join first!";
				if (IsInvoking("ClearMessage")) { CancelInvoke("ClearMessage"); }
				Invoke ("ClearMessage", 2f);

				return;
			}

			_currentJoinCount += 1;

			Notifier.SendEventNotification(PlayerJoinedEvent, controlIndex);

			_requiredPlayerCount += 1;
			_joinedPlayers[controlIndex] = true;
		}
	}

	void HandleSelectDropOutEvent() {
		_joinedPlayers[_playerPauseIndex] = false;
		_currentJoinCount -= 1;

		if (_readyPlayers[_playerPauseIndex]) {
			_readyPlayers[_playerPauseIndex] = false;
			_currentReadyCount -= 1;
		}

		Notifier.SendEventNotification(DropOutEvent, _playerPauseIndex);
	}

	void HandleSelectResumeEvent ()
	{
		_paused = false;

		Notifier.SendEventNotification(ResumeGameEvent);
	}

	void HandleStartInputEvent (int controlID)
	{
		if (_joinedPlayers[controlID])
		{
			if (!_paused) {
				Time.timeScale = 0;

				_playerPauseIndex = controlID;
				_paused = true;

				Notifier.SendEventNotification(PauseGameEvent, _playerPauseIndex);
			}
		}
	}

	private void HandleMaxScoreReachedEvent(ZMLobbyScoreController scoreController) {
		_currentReadyCount += 1;
		_readyPlayers[scoreController.PlayerInfo.ID] = true;

		Notifier.SendEventNotification(PlayerReadyEvent, scoreController.PlayerInfo);

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

	void ClearMessage() {
		message.text = "";
	}
}
