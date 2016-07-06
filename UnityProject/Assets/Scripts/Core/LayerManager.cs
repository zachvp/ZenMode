using UnityEngine;
using System.Collections.Generic;
using Core;

public class Layer
{
	public int number { get; set; }
	public string name { get; set; }
	public bool isActive
	{
		get { return _isActive; }
		set 
		{
			if (_isActive != value)
			{
				_isActive = value;
				Notifier.SendEventNotification(OnActiveChanged, this);
			}
		}
	}

	// Passes the layer index in the array, NOT the layer number.
	public EventHandler<Layer> OnActiveChanged;

	private bool _isActive;

	public Layer(int inNumber, bool inIsActive, string inName)
	{
		number = inNumber;
		_isActive = inIsActive;
		name = inName;
	}
}

public static class LayerManager
{
	// TODO: Use map to add new scene configurations

	public static EventHandler<Layer> OnLayerChangeActive;

	public static Layer[] Layers { get { return _currentConfiguration._layers.ToArray(); } }
	public static int LayerCount { get { return _currentConfiguration != null ? _currentConfiguration._layers.Count : 0; } }

	private const string UNASSIGNED = "";
	private const uint MAX_LAYER_COUNT = 32;

	private static Dictionary<SceneWrapper, LayerConfiguration> _layerConfigurations;

	private static LayerConfiguration _currentConfiguration
	{ get { return LayerManager.GetConfigurationForScene(SceneManagerEditor.CurrentScene); } }

	public static void Init()
	{
		var numScenes = SceneManagerEditor.SceneCount;

		LayerManager.Clear();

		LayerManager._layerConfigurations = new Dictionary<SceneWrapper, LayerConfiguration>(numScenes);

		for (int i = 0; i < numScenes; ++i)
		{
			var configuration = new LayerConfiguration();

			_layerConfigurations.Add(SceneManagerEditor.GetSceneAtIndex(i), configuration);
		}

		LayerManager.AddActiveScene();
	}

	public static void Clear()
	{
		if (_layerConfigurations != null)
		{
			_layerConfigurations.Clear();
			_layerConfigurations = null;
		}

		OnLayerChangeActive = null;
	}

	public static void AddActiveScene()
	{
		var configuration = GetConfigurationForScene(SceneManagerEditor.CurrentScene);

		LayerConfiguration.Init(ref configuration);

		Debug.LogFormat("LayerManager: Initializing configuration for current scene {0}",
			SceneManagerEditor.CurrentScene.Name);
	}

	private static LayerConfiguration GetConfigurationForScene(SceneWrapper scene)
	{
		LayerConfiguration outConfiguration = null;

		if (LayerManager.IsEmpty())
		{
			LayerManager.Init();
		}

		if (scene.IsNull || !_layerConfigurations.TryGetValue(scene, out outConfiguration))
		{
			Debug.LogErrorFormat("LayerManager: Unable to get configuration for scene {0}", scene.Name);
		}

		return outConfiguration;
	}

	public static bool IsEmpty()
	{
		return _layerConfigurations == null || _layerConfigurations.Count == 0;
	}

	public static bool IsSceneInitialized(SceneWrapper scene)
	{
		var configuration = LayerManager.GetConfigurationForScene(scene);

		return configuration.IsInitialized();
	}

	public static void SetLayerObjectsActive(Layer layer, bool active)
	{
		var objectLayerMappings = LayerManager._currentConfiguration._objectLayerMappings;
		List<GameObject> layerObjects;

		if (objectLayerMappings.TryGetValue(layer, out layerObjects))
		{
			foreach (GameObject layerObject in layerObjects)
			{
				layerObject.SetActive(active);
			}
		}
	}

	private static void HandleOnLayerChangeActive(Layer layer)
	{
		Notifier.SendEventNotification(OnLayerChangeActive, layer);
	}

	private static Layer FindLayerForUnityLayer(int layerNumUnity)
	{
		var layers = LayerManager._currentConfiguration._layers;

		for (int i = 0; i < layers.Count; ++i)
		{
			var layer = layers[i];

			if (layer.number == layerNumUnity)
			{
				return layer;
			}
		}

		return null;
	}

	/// <summary>
	/// Internal class for LayerManager.
	/// </summary>
	private class LayerConfiguration
	{
		public List<Layer> _layers;
		public Dictionary<Layer, List<GameObject>> _objectLayerMappings;

		public static void Init(ref LayerConfiguration configuration)
		{
			LayerConfiguration.InitSceneLayers(ref configuration);
			LayerConfiguration.InitLayerMappings(ref configuration, configuration._layers.Count);
		}

		private static void InitSceneLayers(ref LayerConfiguration configuration)
		{
			configuration._layers = new List<Layer>();

			for (int i = 0; i < 32; ++i)
			{
				var layerName = LayerMask.LayerToName(i);

				if (layerName != UNASSIGNED)
				{
					var layer = new Layer(i , true, layerName);

					layer.OnActiveChanged += HandleOnLayerChangeActive;

					configuration._layers.Add(layer);
				}
			}
		}

		private static void InitLayerMappings(ref LayerConfiguration configuration, int layerCount)
		{
			var sceneObjects = Utilities.GetAllGameObjectsInScene();
			List<GameObject> currentObjectList;

			configuration._objectLayerMappings = new Dictionary<Layer, List<GameObject>>(layerCount);

			for (int i = 0; i < sceneObjects.Length; ++i)
			{
				var sceneObject = sceneObjects[i];
				var objectLayer = FindLayerForUnityLayer(sceneObject.layer);

				Debug.AssertFormat(objectLayer != null, "Unrecognized layer {0} for scene object {1}",
					objectLayer, sceneObject.layer);

				if (configuration._objectLayerMappings.TryGetValue(objectLayer, out currentObjectList))
				{
					// If we already have a list for this layer going, add to it.
					currentObjectList.Add(sceneObject);
				}
				else
				{
					// If we don't have an entry for this layer,
					// create a new one and add the object.
					currentObjectList = new List<GameObject>(1);
					currentObjectList.Add(sceneObject);

					configuration._objectLayerMappings.Add(objectLayer, currentObjectList);
				}
			}
		}

		public void Clear()
		{
			_layers.Clear();
			_layers = null;

			_objectLayerMappings.Clear();
			_objectLayerMappings = null;
		}

		public bool IsInitialized()
		{
			return _layers != null && _objectLayerMappings != null;
		}
	}
}
