using UnityEngine;
using Core;
using System.Collections;
using System.Collections.Generic;

public class FrameInputRecord
{
	public Queue<EventRecord> record;

	public FrameInputRecord(Queue<EventRecord> inputRecord)
	{
		record = inputRecord;
	}
}
	
public class ZMInputRecorder<InputEventNotifier, PlaybackEventArgs> : MonoBehaviour
							where PlaybackEventArgs : EventArgs where InputEventNotifier : EventNotifier
{
	public static EventHandler<PlaybackEventArgs> OnPlaybackBegin;
	public static EventHandler<PlaybackEventArgs> OnPlaybackEnd;

	public InputEventNotifier _inputEventNotifier;

	protected PlaybackEventArgs _playbackEventArgs;

	// Archives record for one frame.
	// Stores all inputs for a frame.
	protected Queue<EventRecord> _frameInputs;

	// Archives all frame inputs.
	protected Queue<FrameInputRecord> _canonicalRecord;

	protected virtual void Awake()
	{
		_frameInputs = new Queue<EventRecord>();
		_canonicalRecord = new Queue<FrameInputRecord>();
	}

	void Start()
	{
		Debug.AssertFormat(_inputEventNotifier != null,
						  "{0}: An instance of EventNotifier has not been allocated. Redefine _inputEventNotifier " +
						  "to be the proper type.",
					 	  Utilities.GetClassNameForObject(this));
	}

	void OnDestroy()
	{
		OnPlaybackBegin = null;
		OnPlaybackEnd = null;
	}

	protected virtual void LateUpdate()
	{
		// All the input events for the frame have been captured. Time to log them.
		// Perform a deep copy of our frame inputs.
		var frameInputs = new Queue<EventRecord>(_frameInputs);
		var frameRecord = new FrameInputRecord(frameInputs);

		_canonicalRecord.Enqueue(frameRecord);
		_frameInputs.Clear();
	}

	public virtual void PlaybackBegin()
	{
		Notifier.SendEventNotification(OnPlaybackBegin, _playbackEventArgs);

		StartCoroutine(PlaybackInputEventsInternal());
	}

	protected virtual void PlaybackEnd()
	{
		Notifier.SendEventNotification(OnPlaybackEnd, _playbackEventArgs);
	}

	private IEnumerator PlaybackInputEventsInternal()
	{
		var canonicalRecord = new Queue<FrameInputRecord>(_canonicalRecord);

		while (canonicalRecord.Count > 0)
		{
			// Get the record for a frame.
			var frameRecord = canonicalRecord.Dequeue();

			// Playback all inputs from that frame.
			while (frameRecord.record.Count > 0)
			{
				var inputRecord = frameRecord.record.Dequeue();
				var args = inputRecord.args;
				var eventHandlerInt = inputRecord.GetHandlerWithType<EventHandler<IntEventArgs>>();

				if (args == null)
				{
					_inputEventNotifier.TriggerEvent(inputRecord.eventHandler);
				}
				else
				{
					_inputEventNotifier.TriggerEvent(eventHandlerInt, args as IntEventArgs);
				}
			}

			yield return null;
		}

		PlaybackEnd();
		yield break;
	}
}
