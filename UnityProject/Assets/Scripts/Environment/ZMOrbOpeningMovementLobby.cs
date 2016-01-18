using UnityEngine;
using ZMPlayer;

public class ZMOrbOpeningMovementLobby : ZMOrbOpeningMovement
{
	protected override void InitData()
	{
		GetComponent<ZMPlayerInfo>().ID = ZMLobbyController.CurrentJoinCount - 1;

		base.InitData ();
	}
}
