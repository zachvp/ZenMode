using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ZMPlayer{
	public class ZMScoreController : MonoBehaviour {
		public Text scoreText;
		public Slider scoreBar;
		public string objectiveMessage = "Get to pedestal!";
		public string maxScoreMessage  = "Winner!";
		public float scoreRate;

		// members
		private float _scoreMax;
		//Dictionary<ZMPlayerInfo.PlayerTag, string> playerNames;

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
		private enum ScoreState  { OUT_OF_ZONE, IN_ZONE };
		private enum ZoneState   { INACTIVE, ACTIVE };
		private enum TargetState { ALIVE, DEAD }
		private enum GoalState   { NEUTRAL, MAX, MAXED }
		private enum PointState  { NEUTRAL, GAINING, LOSING, GARBAGE };

		private ScoreState  _scoreState;
		private ZoneState   _zoneState;
		private TargetState _targetState;
		private GoalState   _goalState;
		private PointState  _pointState;

		void Awake() {
			_scoreMax = ZMScorePool.MaxScore;
			scoreBar.maxValue = _scoreMax;

			_playerInfo = GetComponent<ZMPlayerInfo>();
			_allScoreControllers = new List<ZMScoreController>();

			_scoreState  = ScoreState.OUT_OF_ZONE;
			_zoneState   = ZoneState.ACTIVE;
			_targetState = TargetState.ALIVE;
			_goalState   = GoalState.NEUTRAL;
			_pointState  = PointState.NEUTRAL;

			// Add event handlers
			ZMPlayerController.PlayerDeathEvent   += HandlePlayerDeathEvent;
			ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;

			ZMPedestalController.ActivateEvent   += HandlePedestalActivation;
			ZMPedestalController.DeactivateEvent += HandlePedestalDeactivation;
		}

		void Start () {
			/*playerNames = new Dictionary<ZMPlayerInfo.PlayerTag, string>()
			{
				{ ZMPlayerInfo.PlayerTag.PLAYER_1, "P1" },
				{ ZMPlayerInfo.PlayerTag.PLAYER_2, "P2" },
				{ ZMPlayerInfo.PlayerTag.PLAYER_3, "P3" },
				{ ZMPlayerInfo.PlayerTag.PLAYER_4, "P4" }
			};*/
			//playerNames.TryGetValue (_playerInfo.playerTag, out _playerName);

			GameObject[] scoreObjects = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject scoreObject in scoreObjects) {
				_allScoreControllers.Add(scoreObject.GetComponent<ZMScoreController>());
			}

			scoreBar.handleRect = null;
			SetScore (ZMScorePool.CurrentScorePool);
		}

		void FixedUpdate() {
			// pedestal score checks
			if (IsAbleToScore()) {
				if (_pointState != PointState.GAINING) {
					_pointState = PointState.GAINING;
					scoreBar.SendMessage("VibrateStart");
				}
			} else if (IsBeingDrained()) {
				if (_pointState != PointState.LOSING)
					_pointState = PointState.LOSING;
			} else if (_pointState != PointState.NEUTRAL) {
				_pointState = PointState.NEUTRAL;
				scoreBar.SendMessage("VibrateStop");
			}

			// state handling
			if (_pointState == PointState.GAINING) {
				foreach (ZMSoul soul in _drainingSouls) {
					if (soul.GetZen() - scoreRate > 0) {
						AddToScore(scoreRate);
						soul.AddZen(-scoreRate);
					} else if (soul.GetZen() > 0) {
						AddToScore(scoreRate - soul.GetZen());
						soul.SetZen(0);
						scoreBar.SendMessage("VibrateStop");
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
				if (StopScoreEvent != null) {
					StopScoreEvent(this);
				}
			}

			// player score checks
			if (_totalScore <= 0) {
				if (MinScoreReached != null) {
					MinScoreReached(this);
				}
			}

			if (_totalScore >= _scoreMax && !IsMaxed()) {
				scoreText.text = objectiveMessage;
				_goalState = GoalState.MAX;
			}

			// player score state checks
			if (_goalState == GoalState.MAX) {
				_goalState = GoalState.MAXED;
			}

			if (_goalState == GoalState.MAXED) {
				scoreText.text = maxScoreMessage;

				if (MaxScoreReached != null) {
					MaxScoreReached(this);
				}
			}
		}

		void OnTriggerStay2D(Collider2D collision) {
			if (collision.gameObject.CompareTag(kPedestalTag)) {
				ZMSoul soul = collision.GetComponent<ZMSoul>();

				if (!soul.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
					if (_zoneState == ZoneState.ACTIVE && _targetState == TargetState.ALIVE) {
						_scoreState = ScoreState.IN_ZONE;
						AddSoul(soul);
					}
				}
			}
		}

		void OnTriggerExit2D(Collider2D collision) {
			if (collision.gameObject.CompareTag(kPedestalTag)) {
				_scoreState = ScoreState.OUT_OF_ZONE;
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
			if (_totalScore >= 0.0f && _totalScore < _scoreMax) {
				_totalScore += amount;

				if (_totalScore < 0) _totalScore = 0;
				if (_totalScore > _scoreMax) _totalScore = _scoreMax;

				UpdateUI();
			}
		}


		private void UpdateUI() {
			_totalScore = Mathf.Max(_totalScore, 0);

			float normalizedScore = (_totalScore / _scoreMax) * _scoreMax;
			
			scoreText.text = normalizedScore.ToString(kScoreFormat) + "%";

			scoreBar.value = normalizedScore;

			if (UpdateScoreEvent != null) {
				UpdateScoreEvent(this);
			}
		}


		// conditions
		public bool IsAbleToScore() { return _targetState == TargetState.ALIVE && _zoneState == ZoneState.ACTIVE &&
											 _scoreState == ScoreState.IN_ZONE; }
		public bool IsBeingDrained() { return CanOtherScore() && _scoreState == ScoreState.OUT_OF_ZONE && _totalScore > 0; }

		private bool IsMaxed() { return _goalState == GoalState.MAX || _goalState == GoalState.MAXED; }

		private bool CanOtherScore() {
			foreach (ZMScoreController scoreObject in _allScoreControllers) {
				if (scoreObject != null && scoreObject.IsAbleToScore()) {
					return true;
				}
			}

			return false;
		}

		// event handlers
		private void HandlePlayerDeathEvent (ZMPlayerController playerController) {
			if (playerController.gameObject.Equals(gameObject)) {
				_scoreState = ScoreState.OUT_OF_ZONE;
				_targetState = TargetState.DEAD;
			}
		}

		void HandlePlayerRespawnEvent (ZMPlayerController playerController) {
			if (playerController.gameObject.Equals(gameObject)) {
				_targetState = TargetState.ALIVE;
				_pointState = PointState.NEUTRAL;
			}
		}

		private void HandlePedestalActivation (ZMPedestalController pedestalController) {
			_zoneState = ZoneState.ACTIVE;
		}

		private void HandlePedestalDeactivation (ZMPedestalController pedestalController) {
			_scoreState = ScoreState.OUT_OF_ZONE;
			_zoneState = ZoneState.INACTIVE;

			RemoveSoul(pedestalController);
		}

		private void RemoveSoul(ZMPedestalController pedestalController) {
			ZMSoul soul = pedestalController.GetComponent<ZMSoul>();

			_drainingSouls.Remove(soul);
		}

		private void AddSoul(ZMSoul soul) {
			if (!_drainingSouls.Contains(soul)) {
				_drainingSouls.Add(soul);
			}
		}
	}
}
