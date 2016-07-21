using UnityEngine;
using System.Collections;
using ZMConfiguration;
using ZMPlayer;
using Core;

public class ZMSpawnManager : MonoSingleton<ZMSpawnManager>
{
	[SerializeField] private string respawnTag = Tags.kSpawnpointTag;

	private Transform[] _spawnpoints;

	protected float _respawnDelay;

	protected override void Awake()
	{
		base.Awake();

		_respawnDelay = Constants.STAGE_RESPAWN_TIME;

		ZMPlayerController.OnPlayerDeath += HandlePlayerDeathEvent;
	}

	void Start()
	{
		var spawnPoints = GameObject.FindGameObjectsWithTag(respawnTag);
		_spawnpoints = new Transform[spawnPoints.Length];
		
		for (int i = 0; i < spawnPoints.Length; ++i) { _spawnpoints[i] = spawnPoints[i].transform; }
	}

	protected Vector3 GetFarthestSpawnPosition()
	{
		var maximumDistance = float.MinValue;
		var targetIndex = 0;
		
		for (int i = 0; i < _spawnpoints.Length; i++)
		{
			var point = _spawnpoints[i];
			var distance = 0.0f;
			
			foreach (ZMPlayerController player in ZMPlayerManager.Instance.Players)
			{
				if (player && !player.IsDead())
				{
					distance += Vector3.SqrMagnitude(point.position - player.transform.position);
				}
			}
			
			if (distance > maximumDistance)
			{
				maximumDistance = distance;
				targetIndex = i;
			}
		}
		
		return _spawnpoints[targetIndex].position;
	}

	protected Vector3 GetPlayerSpawnPosition(ZMPlayerInfo info)
	{
		for (int i = 0; i < _spawnpoints.Length; ++i)
		{
			var spawnInfo = _spawnpoints[i].GetComponent<ZMPlayerInfo>();

			if (info == spawnInfo) { return _spawnpoints[i].position; }
		}

		Debug.LogWarning("ZMSpawnManager: Unable to find spawnpoint matching player info.");
		return Vector3.zero;
	}

	protected virtual void HandlePlayerDeathEvent(ZMPlayerInfoEventArgs args)
	{
		var player = ZMPlayerManager.Instance.Players[args.info.ID];;

		Utilities.ExecuteAfterDelay(SpawnPlayer, _respawnDelay, player);
	}

	protected virtual void SpawnPlayer(ZMPlayerController playerController)
	{		
		playerController.Respawn(GetFarthestSpawnPosition());
	}
}
