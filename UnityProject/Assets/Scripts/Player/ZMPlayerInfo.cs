using UnityEngine;
using System;
using System.Collections;

namespace ZMPlayer {
	public class ZMPlayerInfo : MonoBehaviour, IComparable {
		public enum PlayerTag : int
		{
			PLAYER_1,
			PLAYER_2,
			PLAYER_3,
			PLAYER_4
		};
		public PlayerTag playerTag;

		int IComparable.CompareTo(object other) {
			int thisID = (int) this.playerTag;
			int otherID = (int) ((ZMPlayerInfo) other).playerTag;

			if (thisID < otherID) return -1;
			if (thisID > otherID) return 1;
			else { return 0; }
		}
	}
}