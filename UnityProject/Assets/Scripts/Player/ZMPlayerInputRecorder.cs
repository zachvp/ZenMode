using UnityEngine;
using Core;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ZMPlayerInputController))]
public class ZMPlayerInputRecorder : MonoBehaviour
{
	public static EventHandler<ZMPlayerInfo, ZMPlayerInputRecorder> OnPlaybackBegin;
	public static EventHandler<ZMPlayerInfo, ZMPlayerInputRecorder> OnPlaybackEnd;

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

	/*
	public EventHandler<int> OnAttackEvent;
	*/

	void OnDestroy()
	{
		OnPlaybackBegin = null;
		OnPlaybackEnd = null;
	}

	void LateUpdate()
	{
		// All the input events for the frame have been captured. Time to log them.
		FrameInputRecord frameRecord;
		Queue<InputRecord> frameInputs = new Queue<InputRecord>(_frameInputs);

		frameRecord = new FrameInputRecord(frameInputs);

		_canonicalRecord.Enqueue(frameRecord);
		_frameInputs.Clear();

		if (Input.GetKeyDown(KeyCode.P))
		{
			PlaybackInputEvents();
		}
	}

	public void PlaybackInputEvents()
	{
		Notifier.SendEventNotification(OnPlaybackBegin, _playerInfo, this);
		StartCoroutine(PlaybackInputEventsInternal());
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

				_inputEventNotifier.TriggerEvent(inputRecord.entry);
			}

			yield return null;
		}

		Notifier.SendEventNotification(OnPlaybackEnd, _playerInfo, this);
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

	private void HandleAttackEvent(int direction)
	{
		// TODO: Maybe just separate attack into separate calls.
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

public class InputRecord : System.IComparable
{
	// Make this a list/array of objects? Index 0 = no args, index 1 = 1 arg...
	public EventHandler entry;

	public InputRecord(EventHandler recordedEvent)
	{
		entry = recordedEvent;
	}

	public override bool Equals(System.Object other)
	{
		var otherRecord = other as InputRecord;

		if (otherRecord == null) { return false; }

		return entry == otherRecord.entry;
	}

	public override int GetHashCode()
	{
		return entry.GetHashCode();
	}

	public static bool operator ==(InputRecord lhs, InputRecord rhs)
	{
		// Automatically equal if same reference (or both null).
		if (System.Object.ReferenceEquals(lhs, rhs)) { return true; }

		// If one is null, but not both, return false.
		if ((object) lhs == null || (object) rhs == null) { return false; }

		return lhs.entry == rhs.entry;
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
	}
}
