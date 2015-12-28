using UnityEngine;
using System.Collections.Generic;
using InControl;
using Notifications;

namespace ZMPlayer {
	public class ZMPlayerInputController : MonoBehaviour {
		// Player info.
		public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
		private ZMPlayerInfo _playerInfo;	
		private int _playerNumber;

		private bool _inputEnabled;

		// Delegates.
		public EventHandler<ZMPlayerInputController> OnMoveRightEvent;
		public EventHandler<ZMPlayerInputController> OnMoveLeftEvent;
		public EventHandler<ZMPlayerInputController> OnNoMoveEvent;
		public EventHandler<ZMPlayerInputController> OnJumpEvent;
		public EventHandler<ZMPlayerInputController, int> OnAttackEvent;
		public EventHandler<ZMPlayerInputController> OnPlungeEvent;
		public EventHandler<ZMPlayerInputController> OnParryEvent;

		void Awake () {
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
			ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;

			ZMLobbyController.PauseGameEvent += HandlePauseGameEventPlayer;
			ZMLobbyController.ResumeGameEvent += HandleResumeGameEvent;
		}

		void Update () {
			if (_inputEnabled) {
				InputDevice inputDevice = InputManager.Devices[_playerNumber];

				// Handle horizontal movement.
				if (inputDevice.LeftStickX > 0.5f) {
					if (OnMoveRightEvent != null) {
						OnMoveRightEvent(this);
					}
				} else if (inputDevice.LeftStickX < -0.5f) {
					if (OnMoveLeftEvent != null) {
						OnMoveLeftEvent(this);
					}
				} else {
					if (OnNoMoveEvent != null) {
						OnNoMoveEvent(this);
					}
				}

				// Handle jumping.
				if (inputDevice.Action1.WasPressed || 
				    inputDevice.Action3.WasPressed || 
				    inputDevice.Action4.WasPressed) {

					if (OnJumpEvent != null) {
						OnJumpEvent(this);
						inputDevice.Vibrate(0.5f);
					}
				}

				// Handle attacking.
				if (inputDevice.Action2.WasPressed ||
				    inputDevice.LeftBumper.WasPressed || inputDevice.RightBumper.WasPressed ||
				    inputDevice.LeftTrigger.WasPressed || inputDevice.RightTrigger.WasPressed) {
					if (inputDevice.LeftStickX > 0.5f) {
						if (OnAttackEvent != null) {
							OnAttackEvent(this, 1);
						}
					}
					else if (inputDevice.LeftStickX < -0.5f) {
						if (OnAttackEvent != null) {
							OnAttackEvent(this, -1);
						}
					}
					else if (inputDevice.LeftStickY < -0.5f) {
						if (OnPlungeEvent != null) {
							OnPlungeEvent(this);
						}
					} 
					else {
						if (OnAttackEvent != null) {
							OnAttackEvent(this, 0);
						}
					}
				}

				// Handle parrying.
				/*
				if (inputDevice.LeftTrigger.WasPressed || inputDevice.RightTrigger.WasPressed) {
					if (ParryEvent != null) {
						ParryEvent(this);
					}
				}
				*/
			}
		}

		void HandleStartGameEvent ()
		{
			SetEnabled(true);
		}

		void HandlePlayerDeathEvent (ZMPlayerController playerController)
		{
			if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				SetEnabled(false);
				
				Invoke("Enable", Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY ? 5f : 0.75f);
			}
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
	}
}