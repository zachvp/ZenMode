using UnityEngine;
using System.Collections.Generic;

namespace ZMConfiguration
{
	public static class Constants
	{
		public const int MAX_PLAYERS = 4;
	}

	public static class Tags
	{
		public const string kSpawnpointTag = "Spawnpoint";
		public const string kPlayerTag 	= "Player";
	}

	public static class Configuration
	{
		// Stores the keys that belong to player 0, player 1, etc...
		public static readonly HashSet<KeyCode>[] KeyboardOwners = new HashSet<KeyCode>[]
		{
			new HashSet<KeyCode>
			{
				KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
				KeyCode.Q, KeyCode.E,
				KeyCode.Escape
			},
			new HashSet<KeyCode>
			{
				KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.UpArrow, KeyCode.DownArrow,
				KeyCode.RightShift, KeyCode.Slash,
				KeyCode.Return
			}
		};
	}
}