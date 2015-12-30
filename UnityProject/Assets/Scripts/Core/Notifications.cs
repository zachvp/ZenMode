using System;

// Responsible for defining delegates and events.
namespace Core
{
	// Defines common delegates.
	public delegate void EventHandler();
    public delegate void EventHandler<T>(T item);
	public delegate void EventHandler<T, U>(T item1, U item2);
	public delegate void EventHandler<T, U, V>(T item1, U item2, V item3);


	public class Notifier
	{
		public static void SendEventNotification(EventHandler eventHandler)
		{
			if (eventHandler != null) { eventHandler(); }
		}

		public static void SendEventNotification<T>(EventHandler<T> eventHandler, T item)
		{
			if (eventHandler != null) { eventHandler(item); }
		}

		public static void SendEventNotification<T, U>(EventHandler<T, U> eventHandler, T item1, U item2)
		{
			if (eventHandler != null) { eventHandler(item1, item2); }
		}

		public static void SendEventNotification<T, U, V>(EventHandler<T, U, V> eventHandler, T item1, U item2, V item3)
		{
			if (eventHandler != null) { eventHandler(item1, item2, item3); }
		}
	}
}
