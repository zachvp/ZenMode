using UnityEngine;
using ZMConfiguration;

public class ZMSpawnManager : MonoBehaviour
{
	private Transform[] _spawnpoints;

	public static ZMSpawnManager Instance
	{
		get
		{
			if (_instance == null) { Debug.LogError("ZMSpawnManager: Instance does not exist in scene."); }
			
			return _instance;
		}
	}
	
	private static ZMSpawnManager _instance;

	void Awake()
	{
		if (_instance != null) { Debug.LogError("ZMSpawnManager: Another instance already exists in the scene."); }
		
		_instance = this;
	}

	void Start()
	{
		var spawnPoints = GameObject.FindGameObjectsWithTag(Tags.kSpawnpointTag);
		_spawnpoints = new Transform[spawnPoints.Length];
		
		for (int i = 0; i < spawnPoints.Length; ++i) { _spawnpoints[i] = spawnPoints[i].transform; }
	}

	public Vector3 GetSpawnPosition()
	{
		var maximumDistance = float.MinValue;
		var targetIndex = 0;
		
		for (int i = 0; i < _spawnpoints.Length; i++)
		{
			var point = _spawnpoints[i];
			var distance = 0.0f;
			
			foreach (ZMPlayerController player in ZMPlayerManager.Instance.Players)
			{
				if (!player.IsDead())
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
}
