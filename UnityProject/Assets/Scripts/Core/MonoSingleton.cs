using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	public static T Instance
	{
		get
		{
			Debug.AssertFormat(_instance != null, "{0}: No instance of MonoSingleton exists in the scene",
												   typeof(MonoSingleton<T>).Name);
			return _instance;
		}
	}

	protected static T _instance;

	protected virtual void Awake()
	{
		Debug.AssertFormat(_instance == null, "{0}: More than one instance of MonoSingleton exists in the scene",
											   typeof(MonoSingleton<T>).Name);
		_instance = this as T;
	}

	protected virtual void OnDestroy()
	{
		_instance = null;
	}
}
