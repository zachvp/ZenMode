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

			if (_textUI == null) { _textMesh = behavior.GetComponent<TextMesh>(); }
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
		public static EventHandler<Layer> OnLayerChangeActive;

		public static Layer[] Layers { get { return _layers.ToArray(); } }
		public static int LayerCount { get { return _layers != null ? _layers.Count : 0; } }

		private const string UNASSIGNED = "";
		private const uint MAX_LAYER_COUNT = 32;

		private static List<Layer> _layers;
		private static Dictionary<Layer, List<GameObject>> _objectLayerMappings;

		public static void Init()
		{
			if (_layers ==  null) { InitSceneLayers(ref _layers); }
			if (_objectLayerMappings == null) { InitLayerMappings(ref _objectLayerMappings, _layers.Count); }
		}

		public static void Clear()
		{
			_layers.Clear();
			_layers = null;

			_objectLayerMappings.Clear();
			_objectLayerMappings = null;

			OnLayerChangeActive = null;
		}

		public static void SetLayerObjectsActive(Layer layer, bool active)
		{
			List<GameObject> layerObjects;

			if (_objectLayerMappings.TryGetValue(layer, out layerObjects))
			{
				foreach (GameObject layerObject in layerObjects)
				{
					layerObject.SetActive(active);
				}
			}
		}


		private static void InitSceneLayers(ref List<Layer> layers)
		{
			layers = new List<Layer>();

			for (int i = 0; i < 32; ++i)
			{
				var layerName = LayerMask.LayerToName(i);

				if (layerName != UNASSIGNED)
				{
					var layer = new Layer(i , true, layerName);

					layer.OnActiveChanged += HandleOnLayerChangeActive;

					layers.Add(layer);
				}
			}
		}

		private static void InitLayerMappings(ref Dictionary<Layer, List<GameObject>> mappings, int layerCount)
		{
			var sceneObjects = Utilities.GetAllGameObjectsInScene();
			List<GameObject> currentObjectList;

			mappings = new Dictionary<Layer, List<GameObject>>(layerCount);

			for (int i = 0; i < sceneObjects.Length; ++i)
			{
				var sceneObject = sceneObjects[i];
				var objectLayer = FindLayerForUnityLayer(sceneObject.layer);

				Debug.AssertFormat(objectLayer != null, "Unrecognized layer {0} for scene object {1}",
								   objectLayer, sceneObject.layer);

				if (mappings.TryGetValue(objectLayer, out currentObjectList))
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

					mappings.Add(objectLayer, currentObjectList);
				}
			}
		}

		private static void HandleOnLayerChangeActive(Layer layer)
		{
			Notifier.SendEventNotification(OnLayerChangeActive, layer);
		}

		private static Layer FindLayerForUnityLayer(int layerNumUnity)
		{
			for (int i = 0; i < _layers.Count; ++i)
			{
				var layer = _layers[i];

				if (layer.number == layerNumUnity)
				{
					return layer;
				}
			}

			return null;
		}

	}
}