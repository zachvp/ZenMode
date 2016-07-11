using System;
using UnityEngine;

namespace Core
{
	public class IntEventArgs : EventArgs
	{
		public int value;
	}

	public class UnityObjectEventArgs : EventArgs
	{
		public UnityEngine.Object arg;
	}

	public class MonoBehaviourEventArgs : EventArgs
	{
		public MonoBehaviour behavior;
	}

	public class MonoBehaviourIntEventArgs : MonoBehaviourEventArgs
	{
		public int value;
	}

	public class MonoBehaviourFloatEventArgs : MonoBehaviourEventArgs
	{
		public float value;
	}

	public class LayerEventArgs : EventArgs
	{
		public LayerEventArgs layer;
	}
}