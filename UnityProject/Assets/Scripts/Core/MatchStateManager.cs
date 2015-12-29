using Notifications;

namespace SriLanka.Match
{
	// Defines the match states.
	public enum MatchState
	{
		NONE	= 0,
		BEGIN	= 1,
		MAIN	= 2,
		END		= 3,
		PAUSE	= 4
	}
	
	// Tracks the state of a match.
	public class MatchStateManager
	{
		// Static read-only MatchState.
		public static MatchState matchState { get; private set; }

		// Events.
		public static event EventHandler OnMatchEnd;
		public static event EventHandler OnPreMatchStart;
		public static event EventHandler OnMatchStart;
		public static event EventHandler OnMatchPause;
        public static event EventHandler OnMatchResume;
		public static event EventHandler<MatchState> OnMatchStateChanged;

		// Signals the period before the match starts.
		public static void StartPreMatch()
		{
			SetState(MatchState.BEGIN);

			Notifier.SendEventNotification(OnPreMatchStart);
		}

		// Starts the Match.
		public static void StartMatch()
		{
			SetState(MatchState.MAIN);

			Notifier.SendEventNotification(OnMatchStart);
		}

		public static void EndMatch()
		{
			SetState(MatchState.END);

			Notifier.SendEventNotification(OnMatchEnd);
		}

		// Pauses the Match state.
		public static void PauseMatch()
		{
			SetState(MatchState.PAUSE);

			Notifier.SendEventNotification(OnMatchPause);
		}

		// Resumes the Match state.
		public static void ResumeMatch()
		{
			SetState(MatchState.MAIN);

			Notifier.SendEventNotification(OnMatchResume);
		}

		// Resets the Match state.
		public static void ResetMatch()
		{
			SetState(MatchState.BEGIN);
			
			ClearEventHandlers();
		}

		public static void TogglePauseMatch()
		{
			if (matchState == MatchState.MAIN)
			{
				PauseMatch();
			}
			else if (matchState == MatchState.PAUSE)
			{
				ResumeMatch();
			}
		}

		// Clears the match state and event listeners. Think of this as a Destroy() lifecycle method.
		public static void Clear()
		{
			SetState(MatchState.NONE);

			ClearEventHandlers();
		}

		// Sets the state and broadcasts the appropriate event.
		private static void SetState(MatchState state)
		{
			matchState = state;

			Notifier.SendEventNotification(OnMatchStateChanged, matchState);
		}

		private static void ClearEventHandlers()
		{
			OnPreMatchStart = null;
			OnMatchStart = null;
			OnMatchEnd = null;
			OnMatchPause = null;
			OnMatchResume = null;
			OnMatchStateChanged = null;
		}
	}
}
