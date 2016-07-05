using UnityEngine;
using System.Collections.Generic;
using ZMConfiguration;

public class ZMUserManager : MonoSingleton<ZMUserManager>
{
	public List<ZMUser> _users { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		Init(Constants.MAX_PLAYERS);
	}

	private void Init(int userCount)
	{
		_users = new List<ZMUser>(userCount);

		for (int i = 0; i < _users.Count; ++i)
		{
			var name = string.Format("P{0}", i + 1);
			var device = ZMDeviceMonitor.Instance._devices[i];
			var user = new ZMUser(name, device);

			_users.Add(user);
		}
	}
}

public class ZMUser
{
	public string _username { get; private set; }
	public ZMInputDevice _device { get; private set; }

	public ZMUser(string name, ZMInputDevice inputDevice)
	{
		_username = name;
		_device = inputDevice;
	}
}
