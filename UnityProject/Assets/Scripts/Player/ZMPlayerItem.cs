using UnityEngine;
using ZMPlayer;

[RequireComponent(typeof(ZMPlayerInfo))]
public class ZMPlayerItem : MonoBehaviour
{
	protected ZMPlayerInfo _playerInfo; public ZMPlayerInfo PlayerInfo { get { return _playerInfo; } }

	protected virtual void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
	}
}
