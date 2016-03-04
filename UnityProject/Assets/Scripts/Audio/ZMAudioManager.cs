using UnityEngine;

// Houses all audio calls.
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioBank))]
public class ZMAudioManager : MonoSingleton<ZMAudioManager>
{
	private AudioSource _audio;

	private AudioBank[] _banks;

	protected override void Awake()
	{
		base.Awake();

		_audio = GetComponent<AudioSource>();
		LoadBanks();
	}

	private void LoadBanks()
	{
		_banks = GetComponents<AudioBank>();
	}

	public void PlayOneShot(string key)
	{
		_audio.PlayOneShot(GetClip(key));
	}

	private AudioClip GetClip(string key)
	{
		AudioClip clip = null;

		for (int i = 0; i < _banks.Length; ++i)
		{
			clip = _banks[i].GetClip(key);
			if (clip != null) { break; }
		}

		Debug.AssertFormat(clip != null, "{0}: Unable to find AudioClip with key {1}", name, key);
		return clip;
	}
}
