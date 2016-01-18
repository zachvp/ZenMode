﻿using UnityEngine;
using System.Collections;
using ZMConfiguration;
using ZMPlayer;
using Core;

public class ZMSpawnManager : MonoBehaviour
{
	[SerializeField] private string respawnTag = Tags.kSpawnpointTag;

	private Transform[] _spawnpoints;

	protected float _respawnDelay;

	public static ZMSpawnManager Instance
	{
		get
		{
			if (_instance == null) { Debug.LogError("ZMSpawnManager: Instance does not exist in scene."); }
			
			return _instance;
		}
	}
	
	private static ZMSpawnManager _instance;

	protected virtual void Awake()
	{
		if (_instance != null) { Debug.LogError("ZMSpawnManager: Another instance already exists in the scene."); }
		
		_instance = this;
		_respawnDelay = Constants.STAGE_RESPAWN_TIME;

		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
	}

	void Start()
	{
		var spawnPoints = GameObject.FindGameObjectsWithTag(respawnTag);
		_spawnpoints = new Transform[spawnPoints.Length];
		
		for (int i = 0; i < spawnPoints.Length; ++i) { _spawnpoints[i] = spawnPoints[i].transform; }
	}

	void OnDestroy()
	{
		_instance = null;
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

	protected virtual void HandlePlayerDeathEvent(ZMPlayerInfo info)
	{
		StartCoroutine(Utilities.ExecuteAfterDelay(SpawnPlayer, _respawnDelay, info.GetComponent<ZMPlayerController>()));
	}

	protected virtual void SpawnPlayer(ZMPlayerController playerController)
	{		
		playerController.Respawn(GetFarthestSpawnPosition());
	}
}