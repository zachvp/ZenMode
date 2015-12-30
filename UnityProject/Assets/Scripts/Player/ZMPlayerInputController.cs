using UnityEngine;
using System.Collections.Generic;
using Notifications;
using Match;

namespace ZMPlayer
{
	public class ZMPlayerInputController : ZMDirectionalInput
	{
		// Player info.
		public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
		private ZMPlayerInfo _playerInfo;

		private int _playerNumber;

		private bool _inputEnabled;

		// Delegates.
		public EventHandler OnMoveRightEvent;
		public EventHandler OnMoveLeftEvent;
		public EventHandler OnNoMoveEvent;
		public EventHandler OnJumpEvent;
		public EventHandler OnPlungeEvent;
		public EventHandler OnParryEvent;

		public EventHandler<int> OnAttackEvent;

		private const float DOT_THRESHOLD = 0.75f;

		protected override void Awake()
		{
			base.Awake();

			_playerInfo = GetComponent<ZMPlayerInfo> ();
			_playerNumber = (int) _playerInfo.playerTag;

			if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
				ZMGameStateController.StartGameEvent += HandleStartGameEvent;
				ZMGameStateController.GameEndEvent += HandleGameEndEvent;
				ZMGameStateController.QuitMatchEvent += HandleQuitMatchEvent;

				MatchStateManager.OnMatchPause += HandlePauseGameEvent;
				MatchStateManager.OnMatchResume += HandleResumeGameEvent;
			} else {
				_inputEnabled = true;
			}

			ZMLobbyController.PauseGameEvent += HandlePauseGameEventPlayer;
			ZMLobbyController.ResumeGameEvent += HandleResumeGameEvent;
		}

		void Update()
		{
			var dotX = Vector2.Dot(_movement, Vector2.right);

			// Handle horizontal movement.
			if (dotX > DOT_THRESHOLD)
			{
				Notifier.SendEventNotification(OnMoveRightEvent);
			}
			else if (dotX < -DOT_THRESHOLD)
			{
				Notifier.SendEventNotification(OnMoveLeftEvent);
			}
			else
			{
				Notifier.SendEventNotification(OnNoMoveEvent);
			}
		}

		// Initialization.
		protected override void AcceptGamepadEvents()
		{
			base.AcceptGamepadEvents();

			var inputManager = ZMInputManager.Instance;

			inputManager.OnAction1 += HandleOnJump;
			inputManager.OnAction3 += HandleOnJump;
			inputManager.OnAction4 += HandleOnJump;

			inputManager.OnAction2 	    += HandleOnAttack;
			inputManager.OnLeftBumper   += HandleOnAttack;
			inputManager.OnLeftTrigger  += HandleOnAttack;
			inputManager.OnRightBumper  += HandleOnAttack;
			inputManager.OnRightTrigger += HandleOnAttack;
		}

		protected override void AcceptKeyboardEvents()
		{
			base.AcceptKeyboardEvents();

			var inputManager = ZMInputManager.Instance;

			inputManager.OnWKey += HandleOnJump;
			inputManager.OnUpArrowKey += HandleOnJump;

			inputManager.OnSKey += HandleOnAttack;
			inputManager.OnEKey += HandleOnAttack;
			inputManager.OnQKey += HandleOnAttack;
			inputManager.OnDownArrowKey += HandleOnAttack;
			inputManager.OnRightShiftKey += HandleOnAttack;
			inputManager.OnSlashKey += HandleOnAttack;
		}

		// Handlers.
		private void HandleOnJump(ZMInput input)
		{
			if (IsCorrectInputControl(input))
			{
				if (input.Pressed)
				{
					Notifier.SendEventNotification(OnJumpEvent);
				}
			}
		}

		private void HandleOnAttack(ZMInput input)
		{
			if (IsCorrectInputControl(input))
			{
				if (input.Pressed)
				{
					var dotX = Vector2.Dot(_movement, Vector2.right);
					var dotY = Vector2.Dot(_movement, Vector2.up);

					if (dotY < -DOT_THRESHOLD)      { Notifier.SendEventNotification(OnPlungeEvent); }
					else if (dotX > DOT_THRESHOLD)  { Notifier.SendEventNotification(OnAttackEvent, 1); }
					else if (dotX < -DOT_THRESHOLD) { Notifier.SendEventNotification(OnAttackEvent, -1); }
					else 							{ Notifier.SendEventNotification(OnAttackEvent, 0); }
				}
			}
		}

		void HandleStartGameEvent()
		{
			SetEnabled(true);
		}

		void HandleQuitMatchEvent ()
		{
			SetEnabled(true);
		}
		
		void HandleGameEndEvent ()
		{
			SetEnabled(false);
		}
		
		void HandleResumeGameEvent ()
		{
			SetEnabled(true);
		}
		
		void HandlePauseGameEvent ()
		{
			SetEnabled(false);
		}

		void HandlePauseGameEventPlayer(int playerIndex)
		{
			SetEnabled(false);
		}

		void Enable() {
			SetEnabled(true);
		}

		private void SetEnabled(bool value) {
			_inputEnabled = value;
		}

		protected override bool IsCorrectInputControl(ZMInput input)
		{
			return input.ID == -1 || input.ID == _playerNumber;
		}
	}
}