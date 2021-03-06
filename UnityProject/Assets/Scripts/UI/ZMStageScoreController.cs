﻿using UnityEngine;
using System.Collections.Generic;
using Core;
using ZMConfiguration;
using ZMPlayer;

public class ZMStageScoreController : ZMScoreController
{
	public static EventHandler<ZMPlayerInfoEventArgs> CanScoreEvent;
	public static EventHandler<ZMPlayerInfoEventArgs> CanDrainEvent;

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
		
		ZMPedestalController.OnDeactivateEvent += HandlePedestalDeactivation;
		
		MatchStateManager.OnMatchEnd += HandleGameEndEvent;
	}

	void FixedUpdate()
	{
		var infoArgs = new ZMPlayerInfoEventArgs(_playerInfo);

		// Score checks.
		if (IsAbleToScore())
		{
			if (_pointState != PointState.GAINING)
			{
				_pointState = PointState.GAINING;
				
				Notifier.SendEventNotification(CanScoreEvent, infoArgs);
			}
		}
		else if (_pointState != PointState.NEUTRAL)
		{
			_pointState = PointState.NEUTRAL;
			
			Notifier.SendEventNotification(OnStopScore, infoArgs);
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
			Notifier.SendEventNotification(CanDrainEvent, infoArgs);
		}
		
		// player score checks
		if (_totalScore <= 0 && _goalState != GoalState.MIN)
		{
			_goalState = GoalState.MIN;

			Notifier.SendEventNotification(OnReachMinScore, infoArgs);
		}
		
		if (_totalScore >= MAX_SCORE && !IsMaxed())
		{
			_goalState = GoalState.MAX;
		}
		
		// Player score state checks
		if (_goalState == GoalState.MAX)
		{
			_goalState = GoalState.MAXED;

			// Assumes total score clamped to max score.
			if (_totalScore == MAX_SCORE) { MatchStateManager.EndMatch(); }

			Notifier.SendEventNotification(OnReachMaxScore, infoArgs);
		}
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		CanScoreEvent	   = null;
		CanDrainEvent      = null;
		OnReachMinScore    = null;
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
		ZMPlayerController.OnPlayerDeath   += HandlePlayerDeathEvent;
		ZMPlayerController.OnPlayerRespawn += HandlePlayerRespawnEvent;
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
	private void HandlePlayerDeathEvent(ZMPlayerInfoEventArgs args)
	{
		if (_playerInfo == args.info)
		{
			_targetState = TargetState.DEAD;
		}
	}
	
	private void HandlePlayerRespawnEvent(ZMPlayerControllerEventArgs args)
	{
		if (args.controller.gameObject.Equals(gameObject))
		{
			_targetState = TargetState.ALIVE;
			_pointState = PointState.NEUTRAL;
		}
	}

	private void HandleGameEndEvent()
	{
		enabled = false;
	}
	
	private void HandlePedestalDeactivation(MonoBehaviourEventArgs args)
	{
		var pedestalController = args.behavior as ZMPedestalController;

		if (_playerInfo == pedestalController.PlayerInfo)
		{
			RemoveSoul(pedestalController);
		}
	}
	
	private void HandleSoulDestroyedEvent(MonoBehaviourEventArgs args)
	{
		var soul = args.behavior as ZMSoul;

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
