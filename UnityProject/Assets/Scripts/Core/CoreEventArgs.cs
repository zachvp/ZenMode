using System;
using UnityEngine;

namespace Core
{
	public abstract class EventArgs
	{
		// Empty
	}

	public class IntEventArgs : EventArgs
	{
		public int value;

		public IntEventArgs(int valueParam)
		{
			value = valueParam;
		}
	}

	public class Vector2EventArgs : EventArgs
	{
		public Vector2 value;

		public Vector2EventArgs() { }

		public Vector2EventArgs(Vector2 valueParam)
		{
			value = valueParam;
		}
	}

	public class UnityObjectEventArgs : EventArgs
	{
		public UnityEngine.Object arg;

		public UnityObjectEventArgs(UnityEngine.Object objectParam)
		{
			arg = objectParam;
		}
	}

	public class MonoBehaviourEventArgs : EventArgs
	{
		public MonoBehaviour behavior;

		public MonoBehaviourEventArgs() { }

		public MonoBehaviourEventArgs(MonoBehaviour behaviorParam)
		{
			behavior = behaviorParam;
		}
	}

	public class MonoBehaviourIntEventArgs : MonoBehaviourEventArgs
	{
		public int value;

		public MonoBehaviourIntEventArgs(MonoBehaviour behaviorParam, int valueParam)
		{
			behavior = behaviorParam;
			value = valueParam;
		}
	}

	public class MonoBehaviourFloatEventArgs : MonoBehaviourEventArgs
	{
		public float value;

		public MonoBehaviourFloatEventArgs(MonoBehaviour behaviorParam, float valueParam)
		{
			behavior = behaviorParam;
			value = valueParam;
		}
	}
}