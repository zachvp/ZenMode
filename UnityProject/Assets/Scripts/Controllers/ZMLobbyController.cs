using UnityEngine;
using System.Collections;
using ZMPlayer;
using Core;
using ZMConfiguration;

public class ZMLobbyController : MonoBehaviour
{
	[SerializeField] private GameObject loadScreen;

	public static int CurrentJoinCount { get { return LobbyPlayer.JoinCount; } }

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

	private int _requiredPlayerCount;

	private bool _paused;

	class LobbyPlayer
	{
		public static int JoinCount { get { return GetStateCount(State.JOINED); } }
		public static int ReadyCount { get { return GetStateCount(State.READY); } }

		private static int[] _stateCounts = new int[(int) State.COUNT];

		private enum State { WAITING, JOINED, READY, DROPPED, COUNT }
		private State _state;

		public void SetWaiting() { UpdateStates(State.WAITING); }
		public void SetJoined()  { UpdateStates(State.JOINED); }
		public void SetReady() 	 { UpdateStates(State.READY); }
		public void SetDropped() { UpdateStates(State.DROPPED); }

		public bool IsNone 	  { get { return _state == State.WAITING; } }
		public bool IsJoined  { get { return _state == State.JOINED; } }
		public bool IsReady   { get { return _state == State.READY; } }
		public bool IsDropped { get { return _state == State.DROPPED; } }

		public static LobbyPlayer[] CreateArray(int size)
		{
			var array = new LobbyPlayer[size];

			for (int i = 0; i < size; ++i) { array[i] = LobbyPlayer.Create(); }

			return array;
		}

		public static LobbyPlayer Create()
		{
			return new LobbyPlayer();
		}

		private void UpdateStates(State current)
		{
			_stateCounts[(int) _state]  -= 1;
			_stateCounts[(int) current] += 1;

			_state = current;
		}

		private static int GetStateCount(State state)
		{
			return _stateCounts[(int) state];
		}
	}

	private LobbyPlayer[] _players;

	void Awake()
	{
		_players = LobbyPlayer.CreateArray(Constants.MAX_PLAYERS);

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
		if (!Utilities.IsValidArrayIndex(_players, id)) { return false; }

		return _players[id].IsJoined;
	}

	private void HandleAnyInputEvent(int controlIndex)
	{
		if (!Utilities.IsValidArrayIndex(_players, controlIndex) || MatchStateManager.IsPause()) { return; }

		if (_players[controlIndex].IsNone)
		{
			_players[controlIndex].SetJoined();

			Notifier.SendEventNotification(OnPlayerJoinedEvent, controlIndex);

			_requiredPlayerCount += 1;

			if (!MatchStateManager.IsPreMatch()) { MatchStateManager.StartPreMatch(); }
		}
	}

	public void PlayerDidDropOut(ZMPlayerInfo info)
	{
		var index = info.ID;

		// Drop player for half a second, then set to waiting.
		_players[index].SetDropped();

		StartCoroutine(Utilities.ExecuteAfterDelay(SetReady, 0.25f, info.ID));

		Notifier.SendEventNotification(OnPlayerDropOut, info);
	}

	private void SetReady(int index)
	{
		_players[index].SetWaiting();
	}

	private void HandleMaxScoreReachedEvent(ZMPlayerInfo info)
	{
		_players[info.ID].SetReady();

		Notifier.SendEventNotification(PlayerReadyEvent, info);

		if (LobbyPlayer.ReadyCount > 1 && LobbyPlayer.ReadyCount == _requiredPlayerCount)
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
