using UnityEngine;
using ZMConfiguration;
using ZMPlayer;

public class ZMCameraOpeningMovement : ZMWaypointMovement
{
	protected override void InitData()
	{
		var startPoints = ZMPlayerManager.Instance.PlayerStartPoints;
		var offset = new Vector3(0, 0, -4);

		_waypointSize = Settings.MatchPlayerCount.value + 1;
		_waypoints = new Transform[_waypointSize];


		for (int i = 0; i < Settings.MatchPlayerCount.value; ++i)
		{
			_waypoints[i] = Instantiate(startPoints[i]) as Transform;
			_waypoints[i].position += offset;
		}

		_waypoints[Settings.MatchPlayerCount.value] = GameObject.FindGameObjectWithTag(Tags.kCameraFocusBase).transform;
	}
}
