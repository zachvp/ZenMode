using System;
using UnityEngine;
using System.Collections.Generic;

// Responsible for defining delegates and events.
namespace Core
{
	public delegate void EventHandler();
	public delegate void EventHandler<EventArgs>(EventArgs args);

	public static class Notifier
	{
		public static void SendEventNotification(EventHandler eventHandler)
		{
			// Temp variable for thread safety.
			var threadsafeHandler = eventHandler;
			if (threadsafeHandler != null) { threadsafeHandler(); }
		}

		public static void SendEventNotification<T>(EventHandler<T> eventHandler, T args)
		{
			// Temp variable for thread safety.
			var threadsafeHandler = eventHandler;
			if (threadsafeHandler != null) { threadsafeHandler(args); }
		}
	}

	public class EventNotifier
	{
		public void TriggerEvent(EventHandler handler)
		{
			Notifier.SendEventNotification(handler);
		}

		public void TriggerEvent<T>(EventHandler<T> handler, T args)
		{
			Notifier.SendEventNotification(handler, args);
		}
	}

	// Event record.
	public class EventRecord
	{
		public CastedType GetHandlerWithType<CastedType>() where CastedType : class
		{
			return _recordedEvent as CastedType;
		}

		public EventHandler eventHandler { get { return _recordedEvent as EventHandler; } }
		public EventArgs args { get; private set; }

		protected object _recordedEvent;

		public EventRecord(EventHandler eventHandler)
		{
			_recordedEvent = eventHandler;
		}

		public EventRecord(object eventHandler, EventArgs eventArgs)
		{
			_recordedEvent = eventHandler;
			args = eventArgs;
		}
	}
		
	public struct NotificationSerial
	{
		public string command;
		public object args;

		public NotificationSerial(string cmd, EventArgs eventArgs)
		{
			command = cmd;
			args = eventArgs;
		}
	}

	// This class serializes the given EventHandlers of any type to a string format that can be read at the other end.
	public static class NotificationsSerializer
	{
		private static Dictionary<string, EventHandler> _registry = new Dictionary<string, EventHandler>();

		public static string RegisterHandlerForSerialization(EventHandler handler)
		{
			string serialized = CreateSerializedCommandFromHandler(handler);
			EventHandler outHandler;

			if (!_registry.TryGetValue(serialized, out outHandler))
			{
				outHandler = handler;
				_registry.Add(serialized, outHandler);
			}

			return serialized;
		}

		public static EventHandler DeserializeHandler(string serializeKey)
		{
			return _registry[serializeKey];
		}

		public static string CreateSerializedCommandFromHandler(EventHandler handler)
		{
			string serialized = "";

			if (handler != null)
			{
				serialized = handler.GetType().ToString();
				serialized = System.Text.RegularExpressions.Regex.Replace(serialized, @"[^a-zA-Z]", string.Empty);

				Debug.LogFormat("serialized: {0}", serialized);
			}

			return serialized;
		}

		public static string CreateSerializedCommandFromHandler<T>(EventHandler<T> handler) where T : EventArgs
		{
			string serialized = "";

			if (handler != null)
			{
				serialized = handler.GetType().ToString();
				serialized = System.Text.RegularExpressions.Regex.Replace(serialized, @"[^a-zA-Z]", string.Empty);

				Debug.LogFormat("serialized type: {0}", serialized);
			}

			return serialized;
		}

		public static string CreateSerializedArgs(EventArgs args)
		{
			return null;
		}
	}
}
