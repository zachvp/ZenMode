using UnityEngine;
using System.Collections.Generic;

public class AudioBank : MonoBehaviour
{
	[SerializeField] private AudioClip[] _clips;

	Dictionary<string, AudioClip> _bank;

	void Awake()
	{
		_bank = new Dictionary<string, AudioClip>(_clips.Length);

		LoadBank(ref _bank);
	}

	public AudioClip GetClip(string key)
	{
		AudioClip clip = null;

		_bank.TryGetValue(key, out clip);

		return clip;
	}

	private void LoadBank(ref Dictionary<string, AudioClip> bank)
	{
		for (int i = 0; i < _clips.Length; ++i)
		{
			var clip = _clips[i];

			_bank.Add(clip.name, clip);
		}
	}
}
