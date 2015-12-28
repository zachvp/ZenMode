using UnityEngine;
using System.Collections.Generic;
using InControl;
using Notifications;

namespace ZMPlayer
{
	public class ZMPlayerInputController : MonoBehaviour
	{
		// Player info.
		public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
		private ZMPlayerInfo _playerInfo;

		private int _playerNumber;

		private bool _inputEnabled;

		private Vector2 _movement;

		// Delegates.
		public EventHandler OnMoveRightEvent;
		public EventHandler OnMoveLeftEvent;
		public EventHandler OnNoMoveEvent;
		public EventHandler OnJumpEvent;
		public EventHandler OnPlungeEvent;
		public EventHandler OnParryEvent;

		public EventHandler<int> OnAttackEvent;

		private const float DOT_THRESHOLD = 0.75f;

		void Awake()
		{
			string playerInfoString;

			_playerInfo = GetComponent<ZMPlayerInfo> ();
			playerInfoString = _playerInfo.playerTag.ToString ();
			_playerNumber = int.Parse (playerInfoString.Substring (playerInfoString.Length - 1)) - 1;

			if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
				ZMGameStateController.StartGameEvent += HandleStartGameEvent;
				ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
				ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;
				ZMGameStateController.GameEndEvent += HandleGameEndEvent;
				ZMGameStateController.QuitMatchEvent += HandleQuitMatchEvent;
			} else {
				_inputEnabled = true;
			}

			ZMPlayerController.PlayerRecoilEvent += HandlePlayerRecoilEvent;
			ZMPlayerController.PlayerStunEvent += HandlePlayerStunEvent;
			ZMPlayerController.PlayerParryEvent += HandlePlayerParryEvent;

			ZMLobbyController.PauseGameEvent += HandlePauseGameEventPlayer;
			ZMLobbyController.ResumeGameEvent += HandleResumeGameEvent;

			AcceptGamepadEvents();
			AcceptKeyboardEvents();
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
		private void AcceptGamepadEvents()
		{
			var inputManager = ZMInputManager.Instance;

			inputManager.OnAction1 += HandleOnJump;
			inputManager.OnAction3 += HandleOnJump;
			inputManager.OnAction4 += HandleOnJump;

			inputManager.OnAction2 	    += HandleOnAttack;
			inputManager.OnLeftBumper   += HandleOnAttack;
			inputManager.OnLeftTrigger  += HandleOnAttack;
			inputManager.OnRightBumper  += HandleOnAttack;
			inputManager.OnRightTrigger += HandleOnAttack;

			inputManager.OnLeftAnalogStickMove += HandleOnMove;
		}

		private void AcceptKeyboardEvents()
		{
			var inputManager = ZMInputManager.Instance;

			inputManager.OnWKey += HandleOnJump;
			inputManager.OnAKey += HandleOnMoveLeft;
			inputManager.OnDKey += HandleOnMoveRight;

			inputManager.OnSKey += HandleOnMoveDown;
			inputManager.OnSKey += HandleOnAttack;
			inputManager.OnEKey += HandleOnAttack;
			inputManager.OnQKey += HandleOnAttack;
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

		private void HandleOnMove(ZMInput input, Vector2 amount)
		{
			if (IsCorrectInputControl(input))
			{
				_movement = amount;
			}
		}

		private void HandleOnMoveLeft(ZMInput input)
		{
			if (IsCorrectInputControl(input))
			{
				if (input.Pressed)
				{
					_movement.x = -1.0f;
				}
				else if (input.Released)
				{
					_movement.x = 0.0f;
				}
			}
		}

		private void HandleOnMoveRight(ZMInput input)
		{
			if (IsCorrectInputControl(input))
			{
				if (input.Pressed)
				{
					_movement.x = 1.0f;
				}
				else if (input.Released)
				{
					_movement.x = 0.0f;
				}
			}
		}

		private void HandleOnMoveUp(ZMInput input)
		{
			if (IsCorrectInputControl(input))
			{
				if (input.Pressed)
				{
					_movement.y = 1.0f;
				}
				else if (input.Released)
				{
					_movement.y = 0.0f;
				}
			}
		}

		private void HandleOnMoveDown(ZMInput input)
		{
			if (IsCorrectInputControl(input))
			{
				if (input.Pressed)
				{
					_movement.y = -1.0f;
				}
				else if (input.Released)
				{
					_movement.y = 0.0f;
				}
			}
		}

		void HandleStartGameEvent()
		{
			SetEnabled(true);
		}
		
		void HandlePlayerParryEvent (ZMPlayerController playerController, float parryTime)
		{
			if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				SetEnabled(false);
				
				Invoke("Enable", parryTime);
			}
		}
		
		void HandlePlayerStunEvent (ZMPlayerController playerController, float stunTime)
		{
			if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				SetEnabled(false);

				Invoke("Enable", stunTime);
			}
		}
		
		void HandlePlayerRecoilEvent (ZMPlayerController playerController, float stunTime)
		{
			if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				SetEnabled(false);
				
				Invoke("Enable", stunTime);
			}
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

		private bool IsCorrectInputControl(ZMInput input)
		{
			return input.ID == -1 || input.ID == _playerNumber;
		}
	}
}