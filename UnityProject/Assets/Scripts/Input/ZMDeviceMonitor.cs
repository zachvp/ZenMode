using UnityEngine;
using InControl;
using System.Collections.Generic;

public class ZMDeviceMonitor : MonoSingleton<ZMDeviceMonitor>
{
	public List<ZMInputDevice> _devices { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		Debug.LogFormat("Devices: {0}", InputManager.Devices.Count);

		Init(InputManager.Devices.Count);
	}

	private void Init(int deviceCount)
	{
		_devices = new List<ZMInputDevice>(deviceCount);

		for (int i = 0; i < _devices.Count; ++i)
		{
			var device = new ZMInputDevice(InputManager.Devices[i]);

			_devices.Add(device);
		}
	}
}

public class ZMInputDevice
{
	public InputDevice _device;

	public ZMInputDevice(InputDevice device)
	{
		_device = device;
	}
}
