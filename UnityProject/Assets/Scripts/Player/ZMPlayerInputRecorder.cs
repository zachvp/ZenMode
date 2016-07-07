using UnityEngine;
using Core;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ZMPlayerInputController))]
public class ZMPlayerInputRecorder : MonoBehaviour
{
	// Overview
	//	Record input from InputController every frame
	//	Stored in class containing EventHandler
	//	In HandleInputEvent() functions, instances of class get stored in queue
	//  In update loop, pop items from queue and add to canonical list
	//  This list stores the action (or lack thereof) every frame
	//  Possible optimization: store number of times no-op happened.
	private ZMPlayerInputController _inputController;

	// Enqueue whenever actual input is received.
	private Queue<InputRecord> _inputRecordQueue;

	// Enqueue every frame.
	private List<EventRecord> _canonicalRecordList;

	void Awake()
	{
		_inputController = GetComponent<ZMPlayerInputController>();
		_inputRecordQueue = new Queue<InputRecord>();
		_canonicalRecordList = new List<EventRecord>();

		_inputController.OnJumpEvent += HandleJumpEvent;
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
					_inputController.SendJumpInput();
				}

				yield return null;
			}
		}

		yield break;
	}

	void HandleJumpEvent()
	{
		var record = new InputRecord(_inputController.OnJumpEvent);

		_inputRecordQueue.Enqueue(record);
	}

//	_inputController.OnMoveRightEvent;
//	_inputController.OnMoveLeftEvent;
//	_inputController.OnNoMoveEvent;
//	_inputController.OnPlungeEvent;
//	_inputController.OnParryEvent;
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
