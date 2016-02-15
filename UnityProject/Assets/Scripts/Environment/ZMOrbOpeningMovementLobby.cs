using UnityEngine;
using ZMPlayer;

[RequireComponent(typeof(ZMPlayerInfo))]
public class ZMOrbOpeningMovementLobby : ZMOrbOpeningMovement
{
	protected override void InitData()
	{
		base.InitData();

		_playerInfo.ID = ZMLobbyPlayerManager.LatestJoinIndex;
	}
}
