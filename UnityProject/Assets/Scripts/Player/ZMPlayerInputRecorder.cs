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
	private Queue<InputRecord> _frameInputs;

	// Archives all frame inputs.
	private Queue<FrameInputRecord> _canonicalRecord;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_inputController = GetComponent<ZMPlayerInputController>();

		_inputEventNotifier = new ZMPlayerInputEventNotifier();
		_frameInputs = new Queue<InputRecord>();
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
		var frameInputs = new Queue<InputRecord>(_frameInputs);
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

				_inputEventNotifier.TriggerEvent(inputRecord.handler);
				_inputEventNotifier.TriggerEvent(inputRecord.handlerInt, inputRecord.args as IntEventArgs);
			}

			yield return null;
		}

		Notifier.SendEventNotification(OnPlaybackEnd, playbackArgs);
		yield break;
	}

	private void HandleOnMoveRightEvent()
	{
		var record = new InputRecord(_inputController._inputEventNotifier.OnMoveRightEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleOnMoveLeftEvent()
	{
		var record = new InputRecord(_inputController._inputEventNotifier.OnMoveLeftEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleNoMoveEvent()
	{
		var record = new InputRecord(_inputController._inputEventNotifier.OnNoMoveEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleJumpEvent()
	{
		var record = new InputRecord(_inputController._inputEventNotifier.OnJumpEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandlePlungeEvent()
	{
		var record = new InputRecord(_inputController._inputEventNotifier.OnPlungeEvent);

		_frameInputs.Enqueue(record);
	}

	private void HandleAttackEvent(IntEventArgs args)
	{
		var record = new InputRecord(_inputController._inputEventNotifier.OnAttackEvent, args);

		_frameInputs.Enqueue(record);
	}
}

public class FrameInputRecord
{
	public Queue<InputRecord> record;

	public FrameInputRecord(Queue<InputRecord> inputRecord)
	{
		record = inputRecord;
	}
}

public class InputRecord
{
	// Make this a list/array of objects? Index 0 = no args, index 1 = 1 arg...
	public EventHandler handler { get { return _recordedEvent as EventHandler; } }
	public EventHandler<IntEventArgs> handlerInt { get { return _recordedEvent as EventHandler<IntEventArgs>; } }

	public EventArgs args { get; private set; }

	private object _recordedEvent;

	public InputRecord(object eventHandler)
	{
		Debug.AssertFormat(eventHandler is EventHandler,
						   "Improper constructor. Attempting to create InputRecord containing arguments." +
						   "Use this class for no arguments");
		_recordedEvent = eventHandler;
	}

	public InputRecord(object eventHandler, EventArgs eventArgs)
	{
		Debug.AssertFormat(!(eventHandler is EventHandler),
						   "Improper constructor. Attempting to create InputRecord without arguments." +
						   "Use this class for EventHandlers with arguments (templated).");

		_recordedEvent = eventHandler;
		args = eventArgs;
		// TODO: Enforce supported types with checks.
	}

	/*public override bool Equals(System.Object other)
	{
		var otherRecord = other as InputRecord;

		if (otherRecord == null) { return false; }

		return command == otherRecord.command;
	}

	public override int GetHashCode()
	{
		return command.GetHashCode();
	}

	public static bool operator ==(InputRecord lhs, InputRecord rhs)
	{
		// Automatically equal if same reference (or both null).
		if (System.Object.ReferenceEquals(lhs, rhs)) { return true; }

		// If one is null, but not both, return false.
		if ((object) lhs == null || (object) rhs == null) { return false; }

		return lhs.command == rhs.command;
	}

	public static bool operator !=(InputRecord lhs, InputRecord rhs)
	{
		return !(lhs == rhs);
	}

	int System.IComparable.CompareTo(object other)
	{
		var otherRecord = (InputRecord) other;
		int result = 0;

		if (otherRecord == null && this != null) { result = 1; }
		else if (otherRecord != null && this == null) { result = -1; }

		return result;
	}*/
}
