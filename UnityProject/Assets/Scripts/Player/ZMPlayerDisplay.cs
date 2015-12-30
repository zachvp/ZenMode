using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ZMPlayerDisplay : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _spriteRendererTemplate;
	[SerializeField] private int emitInterval = 1;

	private SpriteRenderer _renderer;
	private SpriteRenderer _trailRenderer;

	private ParticleSystem _particleSystem;

	private int _emitCount;

	void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
		_particleSystem = GetComponent<ParticleSystem>();
//		_trailObject = ZMEmitObject.Instantiate(_trailObject) as ZMEmitObject;
//
//		_trailObject.transform.SetParent(transform);
//		_trailObject.transform.position = Vector3.zero;
//
//		_trailRenderer = _trailObject.Resource.GetComponent<SpriteRenderer>();

//		if (_trailRenderer == null)
//		{
//			Debug.LogError("ZMPlayerDisplay: Expecting SpriteRender attached to _trailObject ZMEmitObject.");
//		}
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
