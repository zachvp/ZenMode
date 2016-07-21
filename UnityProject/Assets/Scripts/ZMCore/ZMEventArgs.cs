using Core;
using UnityEngine;

// Player info
public class ZMPlayerInfoEventArgs : EventArgs
{
	public ZMPlayerInfo info;

	public ZMPlayerInfoEventArgs() { }

	public ZMPlayerInfoEventArgs(ZMPlayerInfo playerInfo)
	{
		info = playerInfo;
	}
}

public class ZMPlayerInfoFloatEventArgs : ZMPlayerInfoEventArgs
{
	public float value;

	public ZMPlayerInfoFloatEventArgs(ZMPlayerInfo playerInfo, float val)
	{
		info = playerInfo;
		value = val;
	}
}

public class ZMPlayerInfoInputEventArgs : ZMPlayerInfoEventArgs
{
	public ZMInput input;

	public ZMPlayerInfoInputEventArgs(ZMPlayerInfo playerInfo, ZMInput inputParam)
	{
		info = playerInfo;
		input = inputParam;
	}
}

public class ZMPlayerInfoPlayerInputRecorderEventArgs : ZMPlayerInfoEventArgs
{
	public ZMPlayerInputRecorder recorder;

	public ZMPlayerInfoPlayerInputRecorderEventArgs(ZMPlayerInfo playerInfo, ZMPlayerInputRecorder inputRecorder)
	{
		info = playerInfo;
		recorder = inputRecorder;
	}
}

// Player input
public class ZMInputEventArgs : EventArgs
{
	public ZMInput input;

	public ZMInputEventArgs() { }

	public ZMInputEventArgs(ZMInput inputParam)
	{
		input = inputParam;
	}
}

public class ZMInputVector2EventArgs : ZMInputEventArgs
{
	public Vector2 value;

	public ZMInputVector2EventArgs(ZMInput inputParam, Vector2 valueParam)
	{
		input = inputParam;
		value = valueParam;
	}
}

public class ZMInputFloatEventArgs : ZMInputEventArgs
{
	public float value;

	public ZMInputFloatEventArgs(ZMInput inputParam, float valueParam)
	{
		input = inputParam;
		value = valueParam;
	}
}

// Player controller
public class ZMPlayerControllerEventArgs : EventArgs
{
	public ZMPlayerController controller;

	public ZMPlayerControllerEventArgs() { }

	public ZMPlayerControllerEventArgs(ZMPlayerController playerController)
	{
		controller = playerController;
	}
}

public class ZMPlayerControllerFloatEventArgs : ZMPlayerControllerEventArgs
{
	public float value;

	public ZMPlayerControllerFloatEventArgs(ZMPlayerController playerController, float valueParam)
	{
		controller = playerController;
		value = valueParam;
	}
}

// Others
public class ZMMenuOptionEventArgs : EventArgs
{
	public ZMMenuOption option;

	public ZMMenuOptionEventArgs(ZMMenuOption menuOption)
	{
		option = menuOption;
	}
}

public class ZMWaypointMovementEventArgs : EventArgs
{
	public ZMWaypointMovement movement;

	public ZMWaypointMovementEventArgs() { }

	public ZMWaypointMovementEventArgs(ZMWaypointMovement waypointMovement)
	{
		movement = waypointMovement;
	}
}

public class ZMWaypointMovementIntEventArgs : ZMWaypointMovementEventArgs
{
	public int value;

	public ZMWaypointMovementIntEventArgs(ZMWaypointMovement waypointMovement, int valueParam)
	{
		movement = waypointMovement;
		value = valueParam;
	}
}
