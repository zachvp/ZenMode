﻿using UnityEngine;
using ZMPlayer;

[RequireComponent(typeof(ZMPlayerInfo))]
public class ZMOrbOpeningMovement : ZMWaypointMovement
{
	[SerializeField] private string destinationTag;
	
	protected override void InitData()
	{
		var playerInfo = GetComponent<ZMPlayerInfo>();

		_waypointSize = 1;
		_waypoints = new Transform[_waypointSize];

		_waypoints[0] = GetWaypoint(destinationTag, playerInfo);
	}

	private Transform GetWaypoint(string tag, ZMPlayerInfo info)
	{
		var waypoints = GetWaypoints(destinationTag);

		for (int i = 0; i < waypoints.Length; ++i)
		{
			var waypointInfo = waypoints[i].GetComponent<ZMPlayerInfo>();

			if (info == waypointInfo)
			{
				return waypoints[i];
			}
		}

		return null;
	}
}
