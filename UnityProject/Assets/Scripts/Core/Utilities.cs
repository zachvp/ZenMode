using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
	public class CoroutineCallback
	{
		public CoroutineCallback() { }

		public CoroutineCallback(EventHandler finishedCallback)
		{
			OnFinished = finishedCallback;
		}

		public Coroutine coroutine { get; set; }
		public EventHandler OnFinished;
	}

	public class DisplayText
	{
		private Text _textUI;
		private TextMesh _textMesh;

		public DisplayText(MonoBehaviour behavior)
		{
			_textUI = behavior.GetComponent<Text>();

			if (_textUI == null)
			{
				_textMesh = behavior.GetComponent<TextMesh>();
			}
		}

		public string Text
		{
			get
			{
				if (_textUI == null) { return _textMesh.text; }
				else { return _textUI.text; }
			}
			set
			{
				if (_textUI == null) { _textMesh.text = value; }
				else { _textUI.text = value; }
			}
		}

		public Color DisplayColor
		{
			get
			{
				if (_textUI == null) { return _textMesh.color; }
				else { return _textUI.color; }
			}
			set
			{
				if (_textUI == null) { _textMesh.color = value; }
				else { _textUI.color = value; }
			}
		}
	}

	public static class Utilities
	{
		private static MonoBehaviour _monoBehavior;

		public static void Init(MonoBehaviour behavior)
		{
			_monoBehavior = behavior;
		}

		public static Color GetNormalizedColor(float r, float g, float b)
		{
			return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
		}

		public static Color GetRGB(Color lhs, Color rhs)
		{
			return new Color(rhs.r, rhs.g, rhs.b, lhs.a);
		}

		public static void StopDelayRoutine(Coroutine coroutine)
		{
			if (coroutine != null)
			{
				_monoBehavior.StopCoroutine(coroutine);
			}
		}

		// Execute a given method after a delay.
		public static Coroutine ExecuteAfterDelay(EventHandler method, float delay)
		{
			return _monoBehavior.StartCoroutine(ExecuteAfterDelayInternal(method, delay));
		}

		public static Coroutine ExecuteAfterDelay<T0>(EventHandler<T0> method, float delay, T0 param0)
		{
			return _monoBehavior.StartCoroutine(ExecuteAfterDelayInternal(method, delay, param0));
		}

		private static IEnumerator ExecuteAfterDelayInternal(EventHandler method, float delay)
		{
			yield return new WaitForSeconds(delay);

			method();
		}

		private static IEnumerator ExecuteAfterDelayInternal<T0>(EventHandler<T0> method, float delay, T0 param0)
		{
			yield return new WaitForSeconds(delay);

			method(param0);
		}

		// Get all transforms in the root's hierarchy.
		public static Transform[] GetAllInHierarchy(Transform root)
		{
			var transforms = new List<Transform>(root.childCount);

			transforms.Add(root);

			for (int i = 0; i < root.childCount; ++i)
			{
				var child = root.GetChild(i);

				if (child.childCount > 0) { transforms.AddRange(GetAllInHierarchy(child)); }
				else { transforms.Add(child); }
			}

			return transforms.ToArray();
		}

		public static bool IsValidArrayIndex<T>(T[] array, int index, bool warn = false)
		{
			var result = -1 < index && index < array.Length;

			if (warn) { Debug.LogWarningFormat("Utilities: index {0} outside of array {1}", index, array); }

			return result;
		}

		public static void SetVisible(GameObject g, bool visible)
		{
			var renderer = g.GetComponent<Renderer>();
			var graphics = g.GetComponents<MaskableGraphic>();

			for (int i = 0; i < graphics.Length; ++i)
			{
				graphics[i].enabled = visible;
			}

			if (renderer != null) { renderer.enabled = visible; }
		}

		public static void SetEnabledHierarchy(GameObject g, bool enabled)
		{
			SetEnabled(g, enabled);
			SetEnabledChildren(g, enabled);
		}

		public static void SetEnabledChildren(GameObject g, bool enabled)
		{
			for (int i = 0; i < g.transform.childCount; ++i)
			{
				var child = g.transform.GetChild(i);

				SetEnabled(child.gameObject, enabled);
			}
		}

		public static void SetEnabled(GameObject g, bool enabled)
		{
			var components = g.GetComponents<MonoBehaviour>();
			var particles = g.GetComponent<ParticleSystem>();

			for (int i = 0; i < components.Length; ++i)
			{
				var component = components[i];

				// Don't know why it's possible to get Text here since it's not a Monobehaviour
				// so have to do this dumb check...
				if (component.GetType() != typeof(Text)) { component.enabled = enabled; }
			}

			if (particles != null)
			{
				if (enabled) { particles.Play(); }
				else { particles.Pause(); }
			}
		}

		public static GameObject[] GetAllGameObjectsInScene()
		{
			return GameObject.FindObjectsOfType(typeof (GameObject)) as GameObject[];
		}

		// TODO: Fix =/
//		public static UnityEngine.Object[] InstantiateObjects(UnityEngine.Object template, int count)
//		{
//			var result = new UnityEngine.Object[count];
//
//			for (int i = 0; i < result.Length; ++i)
//			{
//				result[i] = UnityEngine.Object.Instantiate(template, Vector3.zero, Quaternion.identity);
//			}
//
//			return result;
//		}
	}

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
}