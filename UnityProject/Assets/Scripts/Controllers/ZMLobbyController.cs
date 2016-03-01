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
	public static EventHandler<ZMPlayerInfo> OnPlayerDropOut;

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

	class LobbyPlayer
	{
		public enum State { NONE, JOINED, READY, DROPPED }
		public State state { get; private set; }
	}

	private LobbyPlayer[] _players;

	void Awake()
	{
		_players = new LobbyPlayer[Constants.MAX_PLAYERS];
		_joinedPlayers = new bool[Constants.MAX_PLAYERS];
		_readyPlayers = new bool[Constants.MAX_PLAYERS];

		Debug.Assert(_instance == null, "ZMLobbyController: More than one instance exists in the scene.");

		_instance = this;

		ZMLobbyScoreController.OnReachMaxScore += HandleMaxScoreReachedEvent;
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
		OnPlayerDropOut 	= null;

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

	public void PlayerDidDropOut(ZMPlayerInfo info)
	{
		var index = info.ID;

		_joinedPlayers[index] = false;
		_currentJoinCount -= 1;

		if (_readyPlayers[index])
		{
			_readyPlayers[index] = false;
			_currentReadyCount -= 1;
		}

		Notifier.SendEventNotification(OnPlayerDropOut, info);
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
