using UnityEngine;
using Core;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ZMPlayerInputController))]
public class ZMPlayerInputRecorder : ZMInputRecorder<ZMPlayerInputEventNotifier, ZMPlayerInfoPlayerInputRecorderEventArgs>
{
	private ZMPlayerInfo _playerInfo;
	private ZMPlayerInputController _inputController;

	protected override void Awake()
	{
		base.Awake();

		_playerInfo = GetComponent<ZMPlayerInfo>();
		_inputController = GetComponent<ZMPlayerInputController>();

		_inputEventNotifier = new ZMPlayerInputEventNotifier();
		_playbackEventArgs = new ZMPlayerInfoPlayerInputRecorderEventArgs(_playerInfo, this);

		_inputController._inputEventNotifier.OnMoveRightEvent += HandleOnMoveRightEvent;
		_inputController._inputEventNotifier.OnMoveLeftEvent += HandleOnMoveLeftEvent;
		_inputController._inputEventNotifier.OnNoMoveEvent += HandleNoMoveEvent;
		_inputController._inputEventNotifier.OnJumpEvent += HandleJumpEvent;
		_inputController._inputEventNotifier.OnPlungeEvent += HandlePlungeEvent;
		_inputController._inputEventNotifier.OnAttackEvent += HandleAttackEvent;
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();

		if (Input.GetKeyDown(KeyCode.P))
		{
			PlaybackBegin();
		}
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
