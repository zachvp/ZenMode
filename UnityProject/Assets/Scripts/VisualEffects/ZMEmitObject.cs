using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZMEmitObject : ZMPlayerItem
{
	[SerializeField] private GameObject resource;

	public GameObject Resource { get { return resource; } }

	public int interval = 10;

	private Color _color;
	private int _currentFrame = 0;

	protected override void Awake()
	{
		base.Awake();

		ZMGameStateController.Instance.GameEndEvent += HandleGameEndEvent;
	}

	void Start()
	{
		_color = _playerInfo.standardColor;
	}

	void HandleGameEndEvent ()
	{
		enabled = false;
	}

	// Update is called once per frame
	void Update ()
	{
		if (_currentFrame > interval)
		{
			GameObject emitObject = GameObject.Instantiate(resource) as GameObject;

			emitObject.transform.position = transform.position;
			emitObject.GetComponent<Text>().color = _color;
			emitObject.transform.SetParent(transform);

			_currentFrame = 0;
		}

		_currentFrame += 1;
	}
}
