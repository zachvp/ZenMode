using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ZMPlayerDisplay : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _spriteRendererTemplate;
	[SerializeField] private int emitInterval = 1;

	private SpriteRenderer _renderer;
	private SpriteRenderer _trailRenderer;
	
	private int _emitCount;

	void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		 _emitCount++;

		if (_emitCount > emitInterval)
		{
			var trail = SpriteRenderer.Instantiate(_spriteRendererTemplate) as SpriteRenderer;

			trail.sprite = _renderer.sprite;
			trail.color = _renderer.color;
			trail.transform.position = transform.position;

			_emitCount = 0;
		}
	}
}
