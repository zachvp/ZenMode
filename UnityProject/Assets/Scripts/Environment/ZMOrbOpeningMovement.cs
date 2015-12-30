using UnityEngine;
using ZMPlayer;

[RequireComponent(typeof(ZMPlayerInfo))]
public class ZMOrbOpeningMovement : ZMWaypointMovement
{
	protected override void InitData()
	{
		var playerInfo = GetComponent<ZMPlayerInfo>();

		_waypointSize = 1;
		_waypoints = new Transform[_waypointSize];

		_waypoints[0] = ZMPlayerManager.Instance.PlayerStartPoints[(int) playerInfo.playerTag];

		Debug.Log(" ");
	}
}
