using UnityEngine;
using System.Collections.Generic;
using InControl;

namespace ZMPlayer {
	public class ZMPlayerInputController : MonoBehaviour {
		// Player info.
		public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
		private ZMPlayerInfo _playerInfo;	
		private int _playerNumber;

		private bool _inputEnabled;

		// Delegates.
		public delegate void MoveRightAction(ZMPlayerInputController playerInputController); 				public static event MoveRightAction MoveRightEvent;
		public delegate void MoveLeftAction(ZMPlayerInputController playerInputController);					public static event MoveRightAction MoveLeftEvent;
		public delegate void NoMoveAction(ZMPlayerInputController playerInputController);					public static event NoMoveAction NoMoveEvent;
		public delegate void JumpAction(ZMPlayerInputController playerInputController);						public static event JumpAction JumpEvent;
		public delegate void AttackAction(ZMPlayerInputController playerInputController, int direction); 	public static event AttackAction AttackEvent;
		public delegate void PlungeAction(ZMPlayerInputController playerInputController);					public static event PlungeAction PlungeEvent;
		public delegate void ParryAction(ZMPlayerInputController playerInputController);					public static event ParryAction ParryEvent;

		void Awake () {
			string playerInfoString;

			_playerInfo = GetComponent<ZMPlayerInfo> ();
			playerInfoString = _playerInfo.playerTag.ToString ();
			_playerNumber = int.Parse (playerInfoString.Substring (playerInfoString.Length - 1)) - 1;

			_inputEnabled = true;

			if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
				ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
				ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;
				ZMGameStateController.GameEndEvent += HandleGameEndEvent;
				ZMGameStateController.QuitMatchEvent += HandleQuitMatchEvent;
			}

			ZMPlayerController.PlayerRecoilEvent += HandlePlayerRecoilEvent;
			ZMPlayerController.PlayerStunEvent += HandlePlayerStunEvent;
			ZMPlayerController.PlayerParryEvent += HandlePlayerParryEvent;
			ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
			ZMLobbyController.PauseGameEvent += HandlePauseGameEventPlayer;
			ZMLobbyController.ResumeGameEvent += HandleResumeGameEvent;
		}

		void OnDestroy() {
			MoveRightEvent = null;
			MoveLeftEvent  = null;
			NoMoveEvent	   = null;
			JumpEvent	   = null;
			AttackEvent	   = null;
			PlungeEvent    = null;
			ParryEvent     = null;
		}

		void Update () {
			if (_inputEnabled) {
				InputDevice inputDevice = InputManager.Devices[_playerNumber];

				// Handle horizontal movement.
				if (inputDevice.LeftStickX > 0.5f) {
					if (MoveRightEvent != null) {
						MoveRightEvent(this);
					}
				} else if (inputDevice.LeftStickX < -0.5f) {
					if (MoveLeftEvent != null) {
						MoveLeftEvent(this);
					}
				} else {
					if (NoMoveEvent != null) {
						NoMoveEvent(this);
					}
				}

				// Handle jumping.
				if (inputDevice.Action1.WasPressed || inputDevice.Action3.WasPressed || inputDevice.Action4.WasPressed) {
					if (JumpEvent != null) {
						JumpEvent(this);
						inputDevice.Vibrate(0.5f);
					}
				}

				// Handle attacking.
				if (inputDevice.Action2.WasPressed ||
				    inputDevice.LeftBumper.WasPressed || inputDevice.RightBumper.WasPressed ||
				    inputDevice.LeftTrigger.WasPressed || inputDevice.RightTrigger.WasPressed) {
					if (Mathf.Abs(inputDevice.LeftStickX) > 0.5f) {
						if (AttackEvent != null) {
							AttackEvent(this, 0);
						}
					}
					else if (inputDevice.LeftStickY < -0.5f) {
						if (PlungeEvent != null) {
							PlungeEvent(this);
						}
					}
					else {
						if (ParryEvent != null) {
							ParryEvent(this);
						}
					}
				}
			}
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