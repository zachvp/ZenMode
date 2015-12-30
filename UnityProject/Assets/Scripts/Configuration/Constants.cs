using UnityEngine;
using System.Collections.Generic;
using Core;

namespace ZMConfiguration
{
	public static class Constants
	{
		public const int MAX_PLAYERS = 4;
	}

	public static class Tags
	{
		public const string kSpawnpointTag 			= "Spawnpoint";
		public const string kPlayerTag 	   			= "Player";
		public const string kPlayerStartPositionTag = "PlayerStartPosition";
		public const string kCameraFocusBase		= "CameraFocusBase";
	}

	public static class Settings
	{
		public static Setting<int> MatchPlayerCount = new Setting<int>("kMatchPlayerCount");
		public static Setting<int[]> LobbyKillcount = new Setting<int[]>("kLobbyKillCount", Constants.MAX_PLAYERS);
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

		public static readonly Color[] PlayerColors = new Color[Constants.MAX_PLAYERS]
		{
			new Color(50, 200, 70),
			new Color(50, 110, 250),
			new Color(255, 145, 50),
			new Color(55, 200, 30),
		};
	}
}