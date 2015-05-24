using UnityEngine;
using System.Collections.Generic;
using InControl;

namespace ZMPlayer {
	public class ZMPlayerInputController : MonoBehaviour {
		// Player info.
		public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
		private ZMPlayerInfo _playerInfo;	
		private int _playerNumber;

		private const float RECOIL_DISABLE_TIME = 1f;

		// Delegates.
		public delegate void MoveRightAction(ZMPlayerInputController playerInputController); 				public static event MoveRightAction MoveRightEvent;
		public delegate void MoveLeftAction(ZMPlayerInputController playerInputController);					public static event MoveRightAction MoveLeftEvent;
		public delegate void NoMoveAction(ZMPlayerInputController playerInputController);					public static event NoMoveAction NoMoveEvent;
		public delegate void JumpAction(ZMPlayerInputController playerInputController);						public static event JumpAction JumpEvent;
		public delegate void AttackAction(ZMPlayerInputController playerInputController, int direction); 	public static event AttackAction AttackEvent;
		public delegate void PlungeAction(ZMPlayerInputController playerInputController);					public static event PlungeAction PlungeEvent;

		void Awake () {
			string playerInfoString;

			_playerInfo = GetComponent<ZMPlayerInfo> ();
			playerInfoString = _playerInfo.playerTag.ToString ();
			_playerNumber = int.Parse (playerInfoString.Substring (playerInfoString.Length - 1)) - 1;

			if (Application.loadedLevel > ZMSceneIndexList.INDEX_LOBBY) {
				ZMGameStateController.PauseGameEvent += HandlePauseGameEvent;
				ZMGameStateController.ResumeGameEvent += HandleResumeGameEvent;
				ZMGameStateController.GameEndEvent += HandleGameEndEvent;
				ZMGameStateController.QuitMatchEvent += HandleQuitMatchEvent;
				ZMPlayerController.PlayerRecoilEvent += HandlePlayerRecoilEvent;
			}
		}

		void HandlePlayerRecoilEvent (ZMPlayerController playerController)
		{
			if (playerController.PlayerInfo.playerTag.Equals(_playerInfo.playerTag)) {
				enabled = false;

				Invoke("Enable", RECOIL_DISABLE_TIME);
			}
		}

		void OnDestroy() {
			MoveRightEvent = null;
			MoveLeftEvent  = null;
			NoMoveEvent	   = null;
			JumpEvent	   = null;
			AttackEvent	   = null;
			PlungeEvent    = null;
		}

		void Update () {
			// Handle horizontal movement.
			if (InputManager.Devices[_playerNumber].LeftStickX > 0.5f) {
				if (MoveRightEvent != null) {
					MoveRightEvent(this);
				}
			} else if (InputManager.Devices[_playerNumber].LeftStickX < -0.5f) {
				if (MoveLeftEvent != null) {
					MoveLeftEvent(this);
				}
			} else {
				if (NoMoveEvent != null) {
					NoMoveEvent(this);
				}
			}

			// Handle jumping.
			if (InputManager.Devices[_playerNumber].Action1.WasPressed) {
				if (JumpEvent != null) {
					JumpEvent(this);
					InputManager.Devices[_playerNumber].Vibrate(0.5f);
				}
			}

			// Handle attacking.
			if (InputManager.Devices[_playerNumber].Action2.WasPressed) {
				if (AttackEvent != null) {
					AttackEvent(this, 0);
				}
			}

			if (InputManager.Devices[_playerNumber].LeftBumper.WasPressed) {
				if (AttackEvent != null) {
					AttackEvent(this, -1);
				}
			}

			if (InputManager.Devices[_playerNumber].RightBumper.WasPressed) {
				if (AttackEvent != null) {
					AttackEvent(this, 1);
				}
			}

			// Handle plunging.
			if (InputManager.Devices[_playerNumber].LeftTrigger.WasPressed ||
			    InputManager.Devices[_playerNumber].RightTrigger.WasPressed ||
			    InputManager.Devices[_playerNumber].Action3.WasPressed) {
				if (PlungeEvent != null) {
					PlungeEvent(this);
				}
			}
		}

		void HandleQuitMatchEvent ()
		{
			enabled = true;
		}
		
		void HandleGameEndEvent ()
		{
			enabled = false;
		}
		
		void HandleResumeGameEvent ()
		{
			enabled = true;
		}
		
		void HandlePauseGameEvent ()
		{
			enabled = false;
		}

		void Enable() {
			enabled = true;
		}
	}
}