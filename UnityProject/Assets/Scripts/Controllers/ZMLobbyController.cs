using UnityEngine;
using ZMPlayer;
using Core;
using ZMConfiguration;

public class ZMLobbyController : MonoBehaviour
{
	[SerializeField] private GameObject loadScreen;

	public static int CurrentJoinCount { get { return _currentJoinCount; } }

	public static EventHandler<int> OnPlayerJoinedEvent;

	public static EventHandler<ZMPlayerInfo> PlayerReadyEvent;
	public static EventHandler<ZMPlayerInfo> DropOutEvent;

	public static ZMLobbyController Instance
	{
		get
		{
			Debug.Assert(_instance != null, "ZMLobbyController: no instance exists in the scene.");

			return _instance;
		}
	}

	private static ZMLobbyController _instance;

	private static int _currentJoinCount;  // i.e. how many players have pressed a button to join
	private static int _currentReadyCount; // i.e. how many players have actually readied up

	private int _requiredPlayerCount;

	private bool _paused;

	private bool[] _joinedPlayers;
	private bool[] _readyPlayers;

	void Awake()
	{
		_joinedPlayers = new bool[Constants.MAX_PLAYERS];
		_readyPlayers = new bool[Constants.MAX_PLAYERS];

		Debug.Assert(_instance == null, "ZMLobbyController: More than one instance exists in the scene.");

		_instance = this;

		ZMLobbyScoreController.OnMaxScoreReached += HandleMaxScoreReachedEvent;

		ZMGameInputManager.AnyInputEvent += HandleAnyInputEvent;
	}

	void Start()
	{
		loadScreen.SetActive(false);
	}

	void OnDestroy()
	{
		OnPlayerJoinedEvent = null;
		PlayerReadyEvent 	= null;
		DropOutEvent	  	= null;

		_instance = null;
	}

	public bool IsPlayerJoined(int id)
	{
		if (!Utilities.IsValidArrayIndex(_joinedPlayers, id)) { return false; }

		return _joinedPlayers[id];
	}

	private void HandleAnyInputEvent(int controlIndex)
	{
		if (!Utilities.IsValidArrayIndex(_joinedPlayers, controlIndex) || MatchStateManager.IsPause()) { return; }

		if (!_joinedPlayers[controlIndex])
		{
			_joinedPlayers[controlIndex] = true;

			Notifier.SendEventNotification(OnPlayerJoinedEvent, controlIndex);

			_requiredPlayerCount += 1;
			_currentJoinCount += 1;

			if (!MatchStateManager.IsPreMatch()) { MatchStateManager.StartPreMatch(); }
		}
	}

	private void HandleSelectDropOutEvent(int index)
	{
		var droppedPlayer = ZMLobbyPlayerManager.Instance.Players[index];

		_joinedPlayers[index] = false;
		_currentJoinCount -= 1;

		if (_readyPlayers[index])
		{
			_readyPlayers[index] = false;
			_currentReadyCount -= 1;
		}

		Notifier.SendEventNotification(DropOutEvent, droppedPlayer.PlayerInfo);
	}

	private void HandleMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		_currentReadyCount += 1;
		_readyPlayers[info.ID] = true;

		Notifier.SendEventNotification(PlayerReadyEvent, info);

		if (_currentReadyCount > 1 && _currentReadyCount == _requiredPlayerCount)
		{
			Invoke("LoadLevel", 0.5f);
			Invoke("ShowLoadScreen", 0.5f);
		}
	}

	private void ShowLoadScreen()
	{
		loadScreen.SetActive(true);
	}

	private void LoadLevel()
	{
		MatchStateManager.Clear();
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_STAGE);
	}
}
