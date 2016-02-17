using UnityEngine;
using System;
using System.Collections.Generic;
using Core;

namespace ZMConfiguration
{
	public static class Constants
	{
		public const int MAX_PLAYERS = 4;

		public const float STAGE_RESPAWN_TIME = 5.0f;
		public const float LOBBY_RESPAWN_TIME = 0.75f;

		// The unit direction of the stage origin "plane" (line, whatever).
		public static readonly Vector2 STAGE_ORIGIN_LINE = new Vector2(1.0f, 0.0f);
	}

	public static class Tags
	{
		public const string kSpawnpointTag 			= "Spawnpoint";
		public const string kPlayerTag 	   			= "Player";
		public const string kPlayerStartPositionTag = "PlayerStartPosition";
		public const string kCameraFocusBase		= "CameraFocusBase";
		public const string kScoreGui				= "ScoreGui";
		public const string kScoreStatus			= "ScoreStatus";
		public const string kOutput					= "Output";
		public const string kMainCamera				= "MainCamera";
		public const string kWarpVolume				= "WarpVolume";
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
			Utilities.GetNormalizedColor(50, 200, 70),
			Utilities.GetNormalizedColor(50, 110, 250),
			Utilities.GetNormalizedColor(255, 145, 50),
			Utilities.GetNormalizedColor(255, 56, 240)
		};

		public static readonly Color[] PlayerLightColors = new Color[Constants.MAX_PLAYERS]
		{
			Utilities.GetNormalizedColor(10, 185, 0),
			Utilities.GetNormalizedColor(0, 120, 255),
			Utilities.GetNormalizedColor(255, 75, 0),
			Utilities.GetNormalizedColor(255, 56, 240)
		};
	}	
}
