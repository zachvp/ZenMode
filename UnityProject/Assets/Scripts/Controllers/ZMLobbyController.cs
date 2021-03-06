using UnityEngine;
using System.Collections;
using ZMPlayer;
using Core;
using ZMConfiguration;

public class ZMLobbyController : MonoSingleton<ZMLobbyController>
{
	[SerializeField] private GameObject loadScreen;

	public static int CurrentJoinCount { get { return LobbyPlayer.JoinCount; } }

	public static EventHandler<IntEventArgs> OnPlayerJoinedEvent;

	public static EventHandler<ZMPlayerInfoEventArgs> PlayerReadyEvent;
	public static EventHandler<ZMPlayerInfoEventArgs> OnPlayerDropOut;

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

		public bool IsWaiting 	  { get { return _state == State.WAITING; } }
		public bool IsJoined  { get { return _state == State.JOINED; } }
		public bool IsReady   { get { return _state == State.READY; } }
		public bool IsDropped { get { return _state == State.DROPPED; } }

		public static LobbyPlayer[] CreateArray(int size)
		{
			var array = new LobbyPlayer[size];

			for (int i = 0; i < size; ++i)
			{
				array[i] = LobbyPlayer.Create();
			}

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

	protected override void Awake()
	{
		base.Awake();

		_players = LobbyPlayer.CreateArray(Constants.MAX_PLAYERS);

		ZMLobbyScoreController.OnReachMaxScore += HandleMaxScoreReachedEvent;
		ZMGameInputManager.AnyInputEvent += HandleAnyInputEvent;
	}

	void Start()
	{
		loadScreen.SetActive(false);
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		OnPlayerJoinedEvent = null;
		PlayerReadyEvent 	= null;
		OnPlayerDropOut 	= null;
	}

	public bool IsPlayerJoined(int id)
	{
		return Utilities.IsValidArrayIndex(_players, id) && _players[id].IsJoined;
	}

	private void HandleAnyInputEvent(IntEventArgs args)
	{
		var controlIndex = args.value;

		var isAbleToJoin = Utilities.IsValidArrayIndex(_players, controlIndex) &&
						   !MatchStateManager.IsPause() &&
						   _players[controlIndex].IsWaiting;

		if (isAbleToJoin)
		{
			_players[controlIndex].SetJoined();

			Notifier.SendEventNotification(OnPlayerJoinedEvent, args);

			_requiredPlayerCount += 1;

			if (!MatchStateManager.IsPreMatch())
			{
				MatchStateManager.StartPreMatch();
			}
		}
	}

	public void PlayerDidDropOut(ZMPlayerInfo info)
	{
		var index = info.ID;
		var args = new ZMPlayerInfoEventArgs(info);

		// Drop player for half a second, then set to waiting.
		_players[index].SetDropped();

		Utilities.ExecuteAfterDelay(SetReady, 0.25f, info.ID);

		Notifier.SendEventNotification(OnPlayerDropOut, args);
	}

	private void SetReady(int index)
	{
		_players[index].SetWaiting();
	}

	private void HandleMaxScoreReachedEvent(ZMPlayerInfoEventArgs args)
	{
		_players[args.info.ID].SetReady();

		Notifier.SendEventNotification(PlayerReadyEvent, args);

		if (LobbyPlayer.ReadyCount > 1 && LobbyPlayer.ReadyCount == _requiredPlayerCount)
		{
			float loadDelay = 0.5f;

			// Set the number of players in the match to the amount of players who have readied up.
			Settings.MatchPlayerCount.value = LobbyPlayer.ReadyCount;

			Utilities.ExecuteAfterDelay(LoadLevel, loadDelay);
			Utilities.ExecuteAfterDelay(ShowLoadScreen, loadDelay);
		}
	}

	private void ShowLoadScreen()
	{
		loadScreen.SetActive(true);
	}

	private void LoadLevel()
	{
		SceneManager.LoadScene(ZMSceneIndexList.INDEX_STAGE);
	}
}
