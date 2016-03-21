using UnityEngine;

// Activates or deactivates attached GameObject.
public class ZMResponder : MonoBehaviour
{
	[SerializeField] protected bool _awakeActive = true;

	protected bool _isActive;

	protected virtual void Awake()
	{
		SetActive(_awakeActive);
	}

	public void Activate()
	{
		SetActive(true);
	}

	public void Deactivate()
	{
		SetActive(false);
	}

	protected void SetActive(bool active)
	{
		_isActive = active;
		gameObject.SetActive(active);
	}
}
