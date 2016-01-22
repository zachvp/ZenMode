using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ZMPlayer;
using Core;
using ZMConfiguration;

public class ZMLobbyController : MonoBehaviour
{
	public GameObject loadScreen;
	public Text message;

	public static int CurrentJoinCount { get { return _currentJoinCount; } }

	public static EventHandler<int> PlayerJoinedEvent;

	public static EventHandler<ZMPlayerInfo> PlayerReadyEvent;
	public static EventHandler<int> DropOutEvent;
	public static EventHandler<int> PauseGameEvent;
	public static EventHandler ResumeGameEvent;

	private static int _currentJoinCount; // i.e. how many  have pressed a button to join
	private static int _currentReadyCount; // i.e. how many have actually readied up

	private int _requiredPlayerCount;

	private bool _paused;
	private int _playerPauseIndex;

	private bool[] _joinedPlayers;
	private bool[] _readyPlayers;

	// pause menu options
	private const int RESUME_OPTION   = 0;
	private const int QUIT_OPTION 	  = 1;

	// TODO: Make this a MonoSingleton.
	void Awake()
	{
		_joinedPlayers = new bool[Constants.MAX_PLAYERS];
		_readyPlayers = new bool[Constants.MAX_PLAYERS];

		message.text = "";

		ZMLobbyScoreController.OnMaxScoreReached += HandleMaxScoreReachedEvent;

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

	void HandleSelectQuitEvent()
	{
		MatchStateManager.Clear();
		Application.LoadLevel(ZMSceneIndexList.INDEX_MAIN_MENU);
	}

	void HandleMainInputEvent(int controlIndex)
	{
		if (!_joinedPlayers[controlIndex])
		{
			_joinedPlayers[_currentJoinCount] = true;
			Notifier.SendEventNotification(PlayerJoinedEvent, _currentJoinCount);

			_requiredPlayerCount += 1;
			_currentJoinCount += 1;
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

	private void HandleMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		_currentReadyCount += 1;
		_readyPlayers[info.ID] = true;

		Notifier.SendEventNotification(PlayerReadyEvent, info);

		if (_currentReadyCount > 1 && _currentReadyCount == _requiredPlayerCount)
		{
			Invoke("LoadLevel", 1.0f);
			Invoke("ShowLoadScreen", 0.5f);
		}
	}

	void ShowLoadScreen()
	{
		loadScreen.SetActive(true);
	}

	void LoadLevel()
	{
		MatchStateManager.Clear();
		Application.LoadLevel(ZMSceneIndexList.INDEX_STAGE);
	}

	void ClearMessage()
	{
		message.text = "";
	}
}
