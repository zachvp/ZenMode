using UnityEngine;
using System.Collections.Generic;
using Core;
using ZMConfiguration;
using ZMPlayer;

public class ZMStageScoreController : ZMScoreController
{
	public static EventHandler<ZMScoreController> CanScoreEvent;
	public static EventHandler<ZMScoreController> CanDrainEvent;

	// States
	private enum ZoneState   { INACTIVE, ACTIVE };
	private enum TargetState { ALIVE, DEAD }
	private enum GoalState   { NEUTRAL, MAX, MAXED, MIN }
	private enum PointState  { NEUTRAL, GAINING, LOSING };
	
	private TargetState _targetState;
	private GoalState   _goalState;
	private PointState  _pointState;

	private List<ZMSoul> _drainingSouls = new List<ZMSoul>();

	private const string kPedestalTag = "Pedestal";

	protected override void Awake()
	{
		base.Awake();

		_targetState = TargetState.ALIVE;
		_goalState   = GoalState.NEUTRAL;
		_pointState  = PointState.NEUTRAL;
		
		ZMSoul.SoulDestroyedEvent += HandleSoulDestroyedEvent;
		
		ZMPedestalController.DeactivateEvent += HandlePedestalDeactivation;
		
		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
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
			
			Notifier.SendEventNotification(OnStopScore, this);
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
			
			Notifier.SendEventNotification(OnMaxScoreReached, _playerInfo);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		CanScoreEvent	   = null;
		CanDrainEvent      = null;
		MinScoreReached    = null;
	}

	void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag(kPedestalTag))
		{
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
	
	void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag(kPedestalTag))
		{
			RemoveSoul(collision.GetComponent<ZMPedestalController>());
		}
	}

	protected override void AcceptPlayerEvents()
	{
		ZMPlayerController.PlayerDeathEvent   += HandlePlayerDeathEvent;
		ZMPlayerController.PlayerRespawnEvent += HandlePlayerRespawnEvent;
	}

	public override void ConfigureItemWithID(int id)
	{
		base.ConfigureItemWithID(id);

		// xD
		SetScore(Settings.MatchPlayerCount.value > 2 ? MAX_SCORE / 2f : MAX_SCORE / Settings.MatchPlayerCount.value);
	}

	public bool IsAbleToScore() { return _targetState == TargetState.ALIVE && _drainingSouls.Count > 0; }
	
	private bool IsMaxed() { return _goalState == GoalState.MAX || _goalState == GoalState.MAXED; }
	
	// event handlers
	private void HandlePlayerDeathEvent(ZMPlayerInfo info)
	{
		if (_playerInfo == info)
		{
			_targetState = TargetState.DEAD;
		}
	}
	
	private void HandlePlayerRespawnEvent(ZMPlayerController playerController)
	{
		if (playerController.gameObject.Equals(gameObject))
		{
			_targetState = TargetState.ALIVE;
			_pointState = PointState.NEUTRAL;
		}
	}

	private void HandleGameEndEvent()
	{
		enabled = false;
	}
	
	private void HandlePedestalDeactivation(ZMPedestalController pedestalController)
	{
		if (_playerInfo == pedestalController.PlayerInfo)
		{
			RemoveSoul(pedestalController);
		}
	}
	
	private void HandleSoulDestroyedEvent(ZMSoul soul)
	{
		RemoveSoul(soul);
	}
	
	private void RemoveSoul(ZMSoul soul)
	{
		_drainingSouls.Remove(soul);
	}
	
	private void RemoveSoul(ZMPedestalController pedestalController)
	{
		ZMSoul soul = pedestalController.GetComponent<ZMSoul>();
		
		if (soul != null)
		{
			soul.SendMessage("SetPulsingOff", SendMessageOptions.DontRequireReceiver);
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
