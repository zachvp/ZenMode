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
}