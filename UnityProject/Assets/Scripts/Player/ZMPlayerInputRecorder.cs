using UnityEngine;
using Core;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ZMPlayerInputController))]
public class ZMPlayerInputRecorder : MonoBehaviour
{
	public static EventHandler<ZMPlayerInfoPlayerInputRecorderEventArgs> OnPlaybackBegin;
	public static EventHandler<ZMPlayerInfoPlayerInputRecorderEventArgs> OnPlaybackEnd;

	public ZMPlayerInputEventNotifier _inputEventNotifier { get; private set; }

	private ZMPlayerInfo _playerInfo;
	private ZMPlayerInputController _inputController;

	// Stores all inputs for a frame.
	private Queue<EventRecord> _frameInputs;

	// Archives all frame inputs.
	private Queue<FrameInputRecord> _canonicalRecord;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_inputController = GetComponent<ZMPlayerInputController>();

		_inputEventNotifier = new ZMPlayerInputEventNotifier();
		_frameInputs = new Queue<EventRecord>();
		_canonicalRecord = new Queue<FrameInputRecord>();

		_inputController._inputEventNotifier.OnMoveRightEvent += HandleOnMoveRightEvent;
		_inputController._inputEventNotifier.OnMoveLeftEvent += HandleOnMoveLeftEvent;
		_inputController._inputEventNotifier.OnNoMoveEvent += HandleNoMoveEvent;
		_inputController._inputEventNotifier.OnJumpEvent += HandleJumpEvent;
		_inputController._inputEventNotifier.OnPlungeEvent += HandlePlungeEvent;
		_inputController._inputEventNotifier.OnAttackEvent += HandleAttackEvent;
	}

	void OnDestroy()
	{
		OnPlaybackBegin = null;
		OnPlaybackEnd = null;
	}

	void LateUpdate()
	{
		// All the input events for the frame have been captured. Time to log them.
		var frameInputs = new Queue<EventRecord>(_frameInputs);
		var frameRecord = new FrameInputRecord(frameInputs);

		_canonicalRecord.Enqueue(frameRecord);
		_frameInputs.Clear();

		if (Input.GetKeyDown(KeyCode.P))
		{
			PlaybackInputEvents();
		}
	}

	public void PlaybackInputEvents()
	{
		var playbackArgs = new ZMPlayerInfoPlayerInputRecorderEventArgs(_playerInfo, this);

		Notifier.SendEventNotification(OnPlaybackBegin, playbackArgs);
		StartCoroutine(PlaybackInputEventsInternal());
	}

	private IEnumerator PlaybackInputEventsInternal()
	{
		var canonicalRecord = new Queue<FrameInputRecord>(_canonicalRecord);
		var playbackArgs = new ZMPlayerInfoPlayerInputRecorderEventArgs(_playerInfo, this);

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

		Notifier.SendEventNotification(OnPlaybackEnd, playbackArgs);
		yield break;
	}

	private void HandleOnMoveRightEvent()
	{
		var record = new EventRecord(_inputController._inputEventNotifier.OnMoveRightEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleOnMoveLeftEvent()
	{
		var record = new EventRecord(_inputController._inputEventNotifier.OnMoveLeftEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleNoMoveEvent()
	{
		var record = new EventRecord(_inputController._inputEventNotifier.OnNoMoveEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleJumpEvent()
	{
		var record = new EventRecord(_inputController._inputEventNotifier.OnJumpEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandlePlungeEvent()
	{
		var record = new EventRecord(_inputController._inputEventNotifier.OnPlungeEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleAttackEvent(IntEventArgs args)
	{
		var inputEvent = _inputController._inputEventNotifier.OnAttackEvent;
		var record = new EventRecord(inputEvent, args);

		_frameInputs.Enqueue(record);
	}
}

public class FrameInputRecord
{
	public Queue<EventRecord> record;

	public FrameInputRecord(Queue<EventRecord> inputRecord)
	{
		record = inputRecord;
	}
}
