using UnityEngine;
using System.Collections.Generic;

namespace ZMPlayer {
	public class ZMPlayerInputController : MonoBehaviour {

		// Player info.
		public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }
		private ZMPlayerInfo _playerInfo;	
		private int _playerNumber;

		// Delegates.
		public delegate void MoveRightAction(ZMPlayerInputController playerInputController);
		public static event MoveRightAction MoveRightEvent;
		public delegate void MoveLeftAction(ZMPlayerInputController playerInputController);
		public static event MoveRightAction MoveLeftEvent;
		public delegate void NoMoveAction(ZMPlayerInputController playerInputController);
		public static event NoMoveAction NoMoveEvent;
		public delegate void JumpAction(ZMPlayerInputController playerInputController);
		public static event JumpAction JumpEvent;
		public delegate void AttackAction(ZMPlayerInputController playerInputController);
		public static event AttackAction AttackEvent;
		public delegate void PlungeAction(ZMPlayerInputController playerInputController);
		public static event PlungeAction PlungeEvent;

		// booleans to treat axes as buttons
		private bool _attackPressed = false;

		void Awake () {
			_playerInfo = GetComponent<ZMPlayerInfo> ();
			string playerInfoString = _playerInfo.playerTag.ToString ();
			_playerNumber = int.Parse(playerInfoString.Substring (playerInfoString.Length - 1));
		}

		void FixedUpdate () {
			// Handle horizontal movement.
			if (Input.GetAxis(PlayerControl ("RUN")) > 0.5f) {
				if (MoveRightEvent != null) {
					MoveRightEvent(this);
				}
			} else if (Input.GetAxis(PlayerControl ("RUN")) < -0.5f) {
				if (MoveLeftEvent != null) {
					MoveLeftEvent(this);
				}
			} else {
				if (NoMoveEvent != null) {
					NoMoveEvent(this);
				}
			}

			// Handle jumping.
			if (Input.GetButtonDown(PlayerControl ("JUMP"))) {
				if (JumpEvent != null) {
					JumpEvent(this);
				}
			}

			// Handle attacking.
			if (Input.GetButtonDown (PlayerControl ("ATTACK"))) {
				if (AttackEvent != null) {
					AttackEvent(this);
				}
			} else if (Input.GetAxisRaw(PlayerControl ("ATTACK")) != 0.0f) {
				if (!_attackPressed) {
					_attackPressed = true;

					if (AttackEvent != null) {
						AttackEvent(this);
					}
				}
			} else if (Input.GetAxisRaw(PlayerControl ("ATTACK")) == 0.0f) {
				_attackPressed = false;
			}

			// Handle plunging.
			if (_playerNumber == 1) {
				// Debug.Log (Input.GetAxis (PlayerControl ("PLUNGE")));
			}
			if (Input.GetAxisRaw(PlayerControl ("PLUNGE")) > 0.0f) {
				if (PlungeEvent != null) {
					PlungeEvent(this);
				}
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

		string PlayerControl (string control) {
			string result = "P" + _playerNumber.ToString () + "_" + control;
			return result;
		}
	}
}