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

	// Enqueue whenever actual input is received.
	private Queue<InputRecord> _inputRecordQueue;

	// Enqueue every frame.
	private List<EventRecord> _canonicalRecordList;

	void Awake()
	{
		_playerInfo = GetComponent<ZMPlayerInfo>();
		_inputController = GetComponent<ZMPlayerInputController>();

		_inputEventNotifier = new ZMPlayerInputEventNotifier();
		_inputRecordQueue = new Queue<InputRecord>();
		_canonicalRecordList = new List<EventRecord>();

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

	void Update()
	{
		var canonicalRecordCount = _canonicalRecordList.Count;
		InputRecord inputRecord = null;
		EventRecord eventRecord = new EventRecord(null);

		if (_inputRecordQueue.Count > 0)
		{
			inputRecord = _inputRecordQueue.Dequeue();
			eventRecord.inputRecord = inputRecord;
		}

		if (canonicalRecordCount > 0)
		{
			var previousRecord = _canonicalRecordList[canonicalRecordCount - 1];

			if (inputRecord == previousRecord.inputRecord)
			{
				// Duplicate records, so increment the count.
				previousRecord.repetitionCount += 1;
			}
			else
			{
				// We have a new entry because our current record does not match our previous.
				_canonicalRecordList.Add(eventRecord);
			}
		}
		else
		{
			// We can't possibly have a repetition since this is the first entry.
			_canonicalRecordList.Add(eventRecord);
		}

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
		var canonicalRecordCopy = new List<EventRecord>(_canonicalRecordList);

		foreach (EventRecord record in canonicalRecordCopy)
		{
			for (int i = 0; i < record.repetitionCount; ++i)
			{
				if (record.inputRecord != null)
				{
					_inputEventNotifier.TriggerEvent(record.inputRecord.entry);
				}

				yield return null;
			}
		}

		Notifier.SendEventNotification(OnPlaybackEnd, _playerInfo, this);
		yield break;
	}

	private void HandleOnMoveRightEvent()
	{
		var record = new InputRecord(_inputEventNotifier.OnMoveRightEvent);

		_inputEventNotifier.OnMoveRightEvent = _inputController._inputEventNotifier.OnMoveRightEvent;
		_inputRecordQueue.Enqueue(record);
	}

	private void HandleOnMoveLeftEvent()
	{
		var record = new InputRecord(_inputEventNotifier.OnMoveLeftEvent);

		_inputEventNotifier.OnMoveLeftEvent = _inputController._inputEventNotifier.OnMoveLeftEvent;
		_inputRecordQueue.Enqueue(record);
	}

	private void HandleNoMoveEvent()
	{
		var record = new InputRecord(_inputEventNotifier.OnNoMoveEvent);

		_inputEventNotifier.OnNoMoveEvent = _inputController._inputEventNotifier.OnNoMoveEvent;
		_inputRecordQueue.Enqueue(record);
	}

	private void HandleJumpEvent()
	{
		var record = new InputRecord(_inputEventNotifier.OnJumpEvent);

		_inputEventNotifier.OnJumpEvent = _inputController._inputEventNotifier.OnJumpEvent;
		_inputRecordQueue.Enqueue(record);
	}

	private void HandlePlungeEvent()
	{
		var record = new InputRecord(_inputEventNotifier.OnPlungeEvent);

		_inputEventNotifier.OnPlungeEvent = _inputController._inputEventNotifier.OnPlungeEvent;
		_inputRecordQueue.Enqueue(record);
	}

	private void HandleAttackEvent(int direction)
	{
		// TODO: Maybe just separate attack into separate calls.
	}
}

public class EventRecord
{
	// The (possibly null) event record.
	public InputRecord inputRecord;

	// How many successive frames this event was recorded.
	public uint repetitionCount;

	public EventRecord(InputRecord record)
	{
		inputRecord = record;
		repetitionCount = 1;
	}
}

public class InputRecord : System.IComparable
{
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
