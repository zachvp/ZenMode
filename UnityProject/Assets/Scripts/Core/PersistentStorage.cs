using UnityEngine;
using System;
using System.Collections.Generic;

namespace Core
{
	public static class PersistentStorage
	{
		private static Dictionary<string, object> settings = new Dictionary<string, object>();

		public static void AddValueForKey(string key, object value)
		{
			try { settings.Add(key, value); }
			catch (ArgumentException) { settings[key] = value; }
		}

		public static ValueType GetValueForKey<ValueType>(string key)
		{
			object value;

			var message = string.Format("PersistentStorage: Invalid key {0} given. please provide a valid key", key);
			bool result = settings.TryGetValue(key, out value);

			if (!result)
			{
				Debug.LogError(message);
			}

//			Debug.Assert(result, message);

			return (ValueType) value;
		}

		public static bool IsValid(string key)
		{
			object value;

			return settings.TryGetValue(key, out value);
		}

		public static void Clear()
		{
			settings.Clear();
		}
	}

	public class Setting<ValueType>
	{
		private string key;
		
		public Setting(string k) { key = k; }

		public Setting(string k, int size)
		{
			key = k;

			if (typeof(ValueType).IsArray)
			{
				Type elementType = typeof(ValueType).GetElementType();
				Array array = Array.CreateInstance(elementType, size);
				
				value = (ValueType)(object) array;
			}
			else
			{
				Debug.LogError("PersistentStorage: Unable to initialize non-array type setting with this constructor.");
			}
		}
		
		public ValueType value
		{
			get { return PersistentStorage.GetValueForKey<ValueType>(key); }
			set { PersistentStorage.AddValueForKey(key, value); }
		}
		
		public bool IsValid()
		{
			return PersistentStorage.IsValid(key);
		}
	}
}