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
		_inputController._inputEventNotifier.OnJumpEvent += HandleJumpEvent;
	}

	/*
	OnMoveRightEvent;
	public EventHandler OnMoveLeftEvent;
	public EventHandler OnNoMoveEvent;
	public EventHandler OnJumpEvent;
	public EventHandler OnPlungeEvent;
	public EventHandler OnParryEvent;

	public EventHandler<int> OnAttackEvent;
	*/

	void OnDestroy()
	{
		OnPlaybackBegin = null;
		OnPlaybackEnd = null;
	}

	void Update()
	{
		if (_inputRecordQueue.Count > 0)
		{
			// We got input last frame, so add it to the canonical record.
			var inputRecord = _inputRecordQueue.Dequeue();
			var eventRecord = new EventRecord(inputRecord);

			_canonicalRecordList.Add(eventRecord);
		}
		else
		{
			var canonicalRecordCount = _canonicalRecordList.Count;

			// We didn't get any input last frame, let's see if we didn't get input two frames ago.
			if (canonicalRecordCount > 1)
			{
				var checkRecord = _canonicalRecordList[canonicalRecordCount - 2];

				if (checkRecord.inputRecord == null)
				{
					// We didn't get any input two frames ago either, so let's increment that count.
					checkRecord.repetitionCount += 1;
				}
				else
				{
					// We did get input two frames ago, but not this time, so let's add a new null record.
					var nullRecord = new EventRecord(null);
					_canonicalRecordList.Add(nullRecord);
				}
			}
			else
			{
				var nullRecord = new EventRecord(null);
				_canonicalRecordList.Add(nullRecord);
			}
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
		List<EventRecord> canonicalRecordCopy = new List<EventRecord>(_canonicalRecordList);

		foreach (EventRecord record in canonicalRecordCopy)
		{
			// TODO: Account for more than one repetition count.
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

	private void HandleJumpEvent()
	{
		var record = new InputRecord(_inputEventNotifier.OnJumpEvent);

		_inputEventNotifier.OnJumpEvent = _inputController._inputEventNotifier.OnJumpEvent;
		_inputRecordQueue.Enqueue(record);
	}

	private void HandleOnMoveRightEvent()
	{
		var record = new InputRecord(_inputEventNotifier.OnMoveRightEvent);

		_inputEventNotifier.OnMoveRightEvent = _inputController._inputEventNotifier.OnMoveRightEvent;
		_inputRecordQueue.Enqueue(record);
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

public class InputRecord
{
	public EventHandler entry;

	public InputRecord(EventHandler recordedEvent)
	{
		entry = recordedEvent;
	}
}
