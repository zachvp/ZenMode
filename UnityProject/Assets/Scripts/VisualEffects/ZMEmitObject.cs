using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZMEmitObject : MonoBehaviour
{
	[SerializeField] private GameObject resource;

	public GameObject Resource { get { return resource; } }

	public Color color;
	public int interval = 10;

	private int _currentFrame = 0;

	void Awake()
	{
		ZMGameStateController.GameEndEvent += HandleGameEndEvent;
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
			emitObject.GetComponent<Text>().color = color;
			emitObject.transform.SetParent(transform);

			_currentFrame = 0;
		}

		_currentFrame += 1;
	}
}
