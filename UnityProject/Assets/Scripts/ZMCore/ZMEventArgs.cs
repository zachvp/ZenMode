using System;
using UnityEngine;

public class ZMPlayerInfoEventArgs : EventArgs
{
	public ZMPlayerInfo info;
}

public class ZMPlayerInfoInputEventArgs : ZMPlayerInfoEventArgs
{
	public ZMInput input;
}

public class ZMInputEventArgs : EventArgs
{
	public ZMInput input;
}

public class ZMInputVector2EventArgs : ZMInputEventArgs
{
	public Vector2 value;
}

public class ZMInputFloatEventArgs : ZMInputEventArgs
{
	public float value;
}

public class ZMMenuOptionEventArgs : EventArgs
{
	public ZMMenuOption option;
}
