using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

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

	public class StoreTransform
	{
		public Vector3 position;
		public Quaternion rotation;
		public Vector3 localScale;

		public StoreTransform(Transform transform)
		{
			position = transform.position;
			rotation = transform.rotation;
			localScale = transform.localScale;
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

		// Usage example if variable is named "testVariable":
		// 	Utilities.GetVariableName(() => objectInstance))
		public static string GetVariableName<T>(Expression<Func<T>> variableExpression)
		{
			var expressionBody = (MemberExpression) variableExpression.Body;
			return expressionBody.Member.Name;
		}

		public static string GetClassNameForObject(object objectInstance)
		{
			var temp = objectInstance;
			string className;

			Debug.AssertFormat(objectInstance != null,
							   "GetClassNameForObject: object with variable name {0} is null",
							   Utilities.GetVariableName(() => objectInstance));

			className = temp.GetType().Name;

			return className;
		}

		// All filepaths here are relative to Unity's "Assets/Resources" folder.
		public static class FileIO
		{
			public static string[] ReadAllLinesFromFile(string filepath)
			{
				var fileContents = Resources.Load(filepath) as TextAsset;
				string[] result = null;

				if (fileContents == null)
				{
					Debug.LogErrorFormat("Utilities.FileIO: Unable to load file at path: {0}", filepath);
				}
				else
				{
					var reader = new StringReader(fileContents.text);
					var lines = new List<string>();
					var line = reader.ReadLine();

					while (line != null)
					{
						lines.Add(line);
						line = reader.ReadLine();
					}

					result = lines.ToArray();
				}

				return result;
			}
		}
	}

	public static class Environment
	{
		public static RaycastHit2D CheckLeft(Vector3 position, Vector2 offset, float distance, LayerMask mask)
		{
			Vector2 rayOrigin = new Vector2(position.x, position.y);
			Vector2 rayDirection = new Vector2(-1.0f, 0.0f);

			rayOrigin += offset;

			return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
		}

		public static RaycastHit2D CheckRight(Vector3 position, Vector2 offset, float distance, LayerMask mask)
		{
			Vector2 rayOrigin = new Vector2(position.x, position.y);
			Vector2 rayDirection = new Vector2(1.0f, 0.0f);

			rayOrigin += offset;

			return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
		}

		public static RaycastHit2D CheckBelow(Vector3 position, Vector2 offset, float distance, LayerMask mask)
		{
			Vector2 rayOrigin = new Vector2(position.x, position.y);
			Vector2 rayDirection = new Vector2(0.0f, -1.0f);

			rayOrigin += offset;
			Debug.DrawRay(rayOrigin, rayDirection, Color.yellow);

			return Physics2D.Raycast(rayOrigin, rayDirection, distance, mask);
		}
	}
}