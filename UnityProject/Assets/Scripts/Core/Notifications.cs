using UnityEngine;
using System.Runtime.Serialization;

// Responsible for defining delegates and events.
namespace Core
{
	public delegate void EventHandler();
	public delegate void EventHandler<EventArgs>(EventArgs args);

	public struct EventArgWrapper
	{
		
	}

	public struct EventHandlerWrapper
	{
		public EventHandler handler;
	}

	public class Notifier
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

	// This class serializes the given EventHandlers of any type to a string format that can be read at the other end.
	public static class NotificationsSerializer
	{
		public static string CreateSerializedEventHandler(EventHandler eventHandler)
		{
			string result = null;

			if (eventHandler != null)
			{
				result = JsonUtility.ToJson(eventHandler);
			}

			return result;
		}

		/*public static string CreateSerializedEventHandler<T>(EventHandler<T> eventHandler, T args)
		{
			string result = null;

			if (eventHandler != null)
			{
				result = Utilities.GetClassNameForObject(eventHandler);
			}

			return result;
		}*/
	}
}
