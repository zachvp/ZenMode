using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ZMPlayer{
	public class ZMScoreController : MonoBehaviour {
		public Slider scoreBar;
		private const float SCORE_RATE = 0.85f;

		private const float MAX_SCORE = 1000.0f;

		// Events
		public delegate void MaxScoreAction(ZMScoreController scoreController);
		public static event MaxScoreAction MaxScoreReached;

		public delegate void MinScoreAction(ZMScoreController scoreController);
		public static event MinScoreAction MinScoreReached;

		public delegate void UpdateScoreAction(ZMScoreController scoreController);
		public static event UpdateScoreAction UpdateScoreEvent;

		public delegate void CanScoreAction(ZMScoreController scoreController);
		public static event CanScoreAction CanScoreEvent;

		public delegate void CanDrainAction(ZMScoreController scoreController);
		public static event CanDrainAction CanDrainEvent;

		public delegate void StopScoreAction(ZMScoreController scoreController);
		public static event StopScoreAction StopScoreEvent;

		// References
		private ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
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
		private enum GoalState   { NEUTRAL, MAX, MAXED, ELIMINATED }
		private enum PointState  { NEUTRAL, GAINING, LOSING };

		private TargetState _targetState;
		private GoalState   _goalState;
		private PointState  _pointState;

		void Awake() {
			_totalScore = 0;
			scoreBar.maxValue = MAX_SCORE;

			_playerInfo = GetComponent<ZMPlayerInfo>();
			_allScoreControllers = new List<ZMScoreController>();

			_targetState = TargetState.ALIVE;
			_goalState   = GoalState.NEUTRAL;
			_pointState  = PointState.NEUTRAL;

			// Add event handlers
			ZMPlayerController.PlayerDeathEvent   += HandlePlayerDeathEvent;
			ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;

			ZMSoul.SoulDestroyedEvent += HandleSoulDestroyedEvent;

			ZMPedestalController.DeactivateEvent += HandlePedestalDeactivation;
		}

		void Start () {
			GameObject[] scoreObjects = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject scoreObject in scoreObjects) {
				_allScoreControllers.Add(scoreObject.GetComponent<ZMScoreController>());
			}



			scoreBar.handleRect = null;
			scoreBar.maxValue = MAX_SCORE;

			// xD
			SetScore (MAX_SCORE / GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<ZMPlayerManager>().NumPlayers);
		}

		void FixedUpdate() {
			// pedestal score checks
			if (IsAbleToScore()) {
				if (_pointState != PointState.GAINING) {
					_pointState = PointState.GAINING;
					// scoreBar.SendMessage("VibrateStart");
				}
			} else if (_pointState != PointState.NEUTRAL) {
				_pointState = PointState.NEUTRAL;
				// scoreBar.SendMessage("VibrateStop");
			}

			// state handling
			if (_pointState == PointState.GAINING) {
				foreach (ZMSoul soul in _drainingSouls) {
					if (soul.GetComponent<ZMPedestalController>().IsDiabled()) continue;

					if ((soul.GetZen() - SCORE_RATE) > 0) {
						AddToScore(SCORE_RATE);
						soul.AddZen(-SCORE_RATE);
					} else if (soul.GetZen() > 0) {
						AddToScore(SCORE_RATE - soul.GetZen());
						soul.SetZen(0);
						//scoreBar.SendMessage("VibrateStop");
					}
				}

				if (CanScoreEvent != null) {
					CanScoreEvent(this);
				}
			} else if (_pointState == PointState.LOSING) {
				if (CanDrainEvent != null) {
					CanDrainEvent(this);
				}

			} else if (_pointState == PointState.NEUTRAL) {
				//scoreBar.SendMessage("VibrateStop");

				if (StopScoreEvent != null) {
					StopScoreEvent(this);
				}
			}

			// player score checks
			if (_totalScore <= 0 && _goalState != GoalState.ELIMINATED) {
				_goalState = GoalState.ELIMINATED;

				if (MinScoreReached != null) {
					MinScoreReached(this);
				}
			}

			if (_totalScore >= MAX_SCORE && !IsMaxed()) {
				_goalState = GoalState.MAX;
			}

			// player score state checks
			if (_goalState == GoalState.MAX) {
				_goalState = GoalState.MAXED;
			}

			if (_goalState == GoalState.MAXED) {
				if (MaxScoreReached != null) {
					MaxScoreReached(this);
				}
			}
		}

		void OnTriggerStay2D(Collider2D collision) {
			if (collision.gameObject.CompareTag(kPedestalTag)) {
				ZMSoul soul = collision.GetComponent<ZMSoul>();
				ZMPedestalController pedestalController = collision.GetComponent<ZMPedestalController>();

				if (!soul.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
					if (pedestalController.IsEnabled() && _targetState == TargetState.ALIVE) {
						AddSoul(soul);
					} else if (!pedestalController.IsEnabled()) {
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
		public void AddToScore(float amount) {
			if (_totalScore >= 0.0f && _totalScore < MAX_SCORE) {
				_totalScore += amount;

				if (_totalScore < 0) _totalScore = 0;
				if (_totalScore > MAX_SCORE) _totalScore = MAX_SCORE;

				UpdateUI();
			}
		}


		private void UpdateUI() {
			_totalScore = Mathf.Max(_totalScore, 0);

			float normalizedScore = (_totalScore / MAX_SCORE) * MAX_SCORE;

			scoreBar.value = normalizedScore;

			if (UpdateScoreEvent != null) {
				UpdateScoreEvent(this);
			}
		}


		// conditions
		public bool IsAbleToScore() { return _targetState == TargetState.ALIVE && _drainingSouls.Count > 0; }

		private bool IsMaxed() { return _goalState == GoalState.MAX || _goalState == GoalState.MAXED; }

		// event handlers
		private void HandlePlayerDeathEvent (ZMPlayerController playerController) {
			if (playerController.gameObject.Equals(gameObject)) {
				_targetState = TargetState.DEAD;
			}
		}

		void HandlePlayerRespawnEvent (ZMPlayerController playerController) {
			if (playerController.gameObject.Equals(gameObject)) {
				_targetState = TargetState.ALIVE;
				_pointState = PointState.NEUTRAL;
			}
		}

		private void HandlePedestalDeactivation (ZMPedestalController pedestalController) {
			if (_playerInfo.playerTag.Equals(pedestalController.PlayerInfo.playerTag)) {
				RemoveSoul(pedestalController);
			}
		}

		void HandleSoulDestroyedEvent (ZMSoul soul)
		{
			RemoveSoul(soul);
		}
		
		private void RemoveSoul(ZMSoul soul) {
			_drainingSouls.Remove(soul);

			/*if (_drainingSouls.Count == 0) {
				scoreBar.SendMessage("VibrateStop");
			}*/
		}

		private void RemoveSoul(ZMPedestalController pedestalController) {
			ZMSoul soul = pedestalController.GetComponent<ZMSoul>();

			RemoveSoul(soul);
		}

		private void AddSoul(ZMSoul soul) {
			if (!_drainingSouls.Contains(soul)) {
				_drainingSouls.Add(soul);
			}
		}
	}
}
