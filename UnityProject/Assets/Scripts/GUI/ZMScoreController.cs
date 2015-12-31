using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMConfiguration;
using Core;

namespace ZMPlayer
{
	public class ZMScoreController : ZMPlayerItem
	{
		// Events
		public static EventHandler<ZMScoreController> MaxScoreReached;
		public static EventHandler<ZMScoreController> MinScoreReached;
		public static EventHandler<ZMScoreController> UpdateScoreEvent;
		public static EventHandler<ZMScoreController> CanScoreEvent;
		public static EventHandler<ZMScoreController> CanDrainEvent;
		public static EventHandler<ZMScoreController> StopScoreEvent;

		private const float SCORE_RATE = 0.5f;
		private const float MAX_SCORE = 1000.0f;

		// References
		private Slider _scoreBar;
		private Text _scoreStatus;
		private List<ZMScoreController> _allScoreControllers;
		private List<ZMSoul> _drainingSouls = new List<ZMSoul>();
		
		// Constants
		private const string kPedestalTag					      = "Pedestal";
		private const string kUpdateScoreInvokeWrapperMethodName  = "UpdateScoreInvokeWrapper";
		private const string kScoreFormat						  = "0.0";

		//private string _playerName;
		private float _totalScore;   public float TotalScore { get { return _totalScore; } set { _totalScore = value; } }

		// States
		private enum ZoneState   { INACTIVE, ACTIVE };
		private enum TargetState { ALIVE, DEAD }
		private enum GoalState   { NEUTRAL, MAX, MAXED, MIN }
		private enum PointState  { NEUTRAL, GAINING, LOSING };

		private TargetState _targetState;
		private GoalState   _goalState;
		private PointState  _pointState;

		protected override void Awake()
		{
			base.Awake();

			_totalScore = 0;

			_allScoreControllers = new List<ZMScoreController>();

			_targetState = TargetState.ALIVE;
			_goalState   = GoalState.NEUTRAL;
			_pointState  = PointState.NEUTRAL;

			// Add event handlers
			ZMPlayerController.PlayerDeathEvent   += HandlePlayerDeathEvent;
			ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;

			ZMSoul.SoulDestroyedEvent += HandleSoulDestroyedEvent;

			ZMPedestalController.DeactivateEvent += HandlePedestalDeactivation;

			ZMGameStateController.GameEndEvent += HandleGameEndEvent;
		}

		void HandleGameEndEvent()
		{
			enabled = false;
		}

		void Start()
		{
			GameObject[] scoreObjects = GameObject.FindGameObjectsWithTag(Tags.kPlayerTag);

			foreach (GameObject scoreObject in scoreObjects)
			{
				_allScoreControllers.Add(scoreObject.GetComponent<ZMScoreController>());
			}

			_scoreBar = ZMScoreDisplayManager.Instance.ScoreSliders[_playerInfo.ID];
			_scoreBar.handleRect = null;
			_scoreBar.maxValue = MAX_SCORE;

			_scoreStatus = ZMScoreDisplayManager.Instance.ScoreStatuses[_playerInfo.ID];

			// xD
			SetScore(Settings.MatchPlayerCount.value > 2 ? MAX_SCORE / 2f : MAX_SCORE / Settings.MatchPlayerCount.value);

			if (_playerInfo.ID >= Settings.MatchPlayerCount.value)
			{
				_scoreStatus.gameObject.SetActive(false);
			}
			else
			{
				_scoreStatus.text = "";
			}
		}

		void FixedUpdate()
		{
			// Score checks.
			if (IsAbleToScore())
			{
				if (_pointState != PointState.GAINING)
				{
					_pointState = PointState.GAINING;

					Notifier.SendEventNotification(CanScoreEvent, this);
				}
			}
			else if (_pointState != PointState.NEUTRAL)
			{
				_pointState = PointState.NEUTRAL;

				Notifier.SendEventNotification(StopScoreEvent, this);
			}

			// state handling
			if (_pointState == PointState.GAINING)
			{
				foreach (ZMSoul soul in _drainingSouls)
				{
					if (soul.GetComponent<ZMPedestalController>().IsDiabled()) continue;

					if ((soul.GetZen() - SCORE_RATE) > 0)
					{
						AddToScore(SCORE_RATE);
						soul.AddZen(-SCORE_RATE);
						soul.SendMessage("SetPulsingOn");
					}
					else if (soul.GetZen() > 0)
					{
						AddToScore(soul.GetZen());
						soul.SetZen(0);
						soul.SendMessage("SetPulsingOff");
					}
				}
			}
			else if (_pointState == PointState.LOSING)
			{
				Notifier.SendEventNotification(CanDrainEvent, this);

			}

			// player score checks
			if (_totalScore <= 0 && _goalState != GoalState.MIN)
			{
				_goalState = GoalState.MIN;

				if (_scoreStatus != null) { _scoreStatus.text = "ELIMINATED!"; }

				Notifier.SendEventNotification(MinScoreReached, this);
			}

			if (_totalScore >= MAX_SCORE && !IsMaxed())
			{
				_goalState = GoalState.MAX;
			}

			// player score state checks
			if (_goalState == GoalState.MAX)
			{
				_goalState = GoalState.MAXED;

				Notifier.SendEventNotification(MaxScoreReached, this);
			}
		}

		void OnTriggerStay2D(Collider2D collision) {
			if (collision.gameObject.CompareTag(kPedestalTag)) {
				ZMSoul soul = collision.GetComponent<ZMSoul>();
				ZMPedestalController pedestalController = collision.GetComponent<ZMPedestalController>();

				if (_playerInfo != soul.PlayerInfo)
				{
					if (pedestalController.IsEnabled() && _targetState == TargetState.ALIVE)
					{
						AddSoul(soul);
					}
					else if (!pedestalController.IsEnabled())
					{
						RemoveSoul(soul);
					}
				}
			}
		}

		void OnTriggerExit2D(Collider2D collision) {
			if (collision.gameObject.CompareTag(kPedestalTag)) {
				RemoveSoul(collision.GetComponent<ZMPedestalController>());
			}
		}

		void OnDestroy() {
			MaxScoreReached    = null;
			MinScoreReached    = null;
			UpdateScoreEvent   = null;
			CanScoreEvent	   = null;
			CanDrainEvent      = null;
			StopScoreEvent	   = null;
		}

		public void SetScore(float newScore) {
			_totalScore = newScore;

			UpdateUI();
		}

		// utility methods
		public void AddToScore(float amount)
		{
			if (_totalScore >= 0.0f && _totalScore < MAX_SCORE)
			{
				_totalScore += amount;

				if (_totalScore < 0) _totalScore = 0;
				if (_totalScore > MAX_SCORE) _totalScore = MAX_SCORE;

				UpdateUI();
			}
		}


		private void UpdateUI()
		{
			_totalScore = Mathf.Max(_totalScore, 0);

			float normalizedScore = (_totalScore / MAX_SCORE) * MAX_SCORE;

			_scoreBar.value = normalizedScore;

			Notifier.SendEventNotification(UpdateScoreEvent, this);
		}


		// conditions
		public bool IsAbleToScore() { return _targetState == TargetState.ALIVE && _drainingSouls.Count > 0; }

		private bool IsMaxed() { return _goalState == GoalState.MAX || _goalState == GoalState.MAXED; }

		// event handlers
		private void HandlePlayerDeathEvent (ZMPlayerController playerController)
		{
			if (playerController.gameObject.Equals(gameObject))
			{
				_targetState = TargetState.DEAD;
			}
		}

		void HandlePlayerRespawnEvent(ZMPlayerController playerController)
		{
			if (playerController.gameObject.Equals(gameObject))
			{
				_targetState = TargetState.ALIVE;
				_pointState = PointState.NEUTRAL;
			}
		}

		private void HandlePedestalDeactivation(ZMPedestalController pedestalController)
		{
			if (_playerInfo == pedestalController.PlayerInfo)
			{
				RemoveSoul(pedestalController);
			}
		}

		void HandleSoulDestroyedEvent(ZMSoul soul)
		{
			RemoveSoul(soul);
		}
		
		private void RemoveSoul(ZMSoul soul) {
			_drainingSouls.Remove(soul);
		}

		private void RemoveSoul(ZMPedestalController pedestalController)
		{
			ZMSoul soul = pedestalController.GetComponent<ZMSoul>();

			if (soul != null)
			{
				soul.SendMessage("SetPulsingOff");
				RemoveSoul(soul);
			}
		}

		private void AddSoul(ZMSoul soul)
		{
			if (!_drainingSouls.Contains(soul))
			{
				_drainingSouls.Add(soul);
			}
		}
	}
}
