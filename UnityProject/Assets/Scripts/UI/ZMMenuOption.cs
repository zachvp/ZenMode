using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Graphic))]
public class ZMMenuOption : MonoBehaviour, IComparable
{
	// Publicly configured index.
	public int Index { get { return _index; } set { _index = value; } }
	public Graphic graphic { get { return _graphic; } }

	private Graphic _graphic;
	private int _index;

	void Awake()
	{
		_graphic = GetComponent<Graphic>();
	}

	public void Configure(int index)
	{
		_index = index;
	}

	public void Hide()
	{
		Debug.LogFormat("ZMMenuOption: Hide");
		_graphic.enabled = false;
	}

	public void Show()
	{
		Debug.LogFormat("ZMMenuOption: Show");
		_graphic.enabled = true;
	}

	public override bool Equals(System.Object other)
	{
		var option = other as ZMMenuOption;

		if (option == null) { return false; }

		return _index == option.Index;
	}

	public override int GetHashCode()
	{
		return _index;
	}

	public static bool operator ==(ZMMenuOption lhs, ZMMenuOption rhs)
	{
		// Automatically equal if same reference (or both null).
		if (System.Object.ReferenceEquals(lhs, rhs)) { return true; }

		// If one is null, but not both, return false.
		if ((object) lhs == null || (object) rhs == null) { return false; }

		return lhs._index == rhs._index;
	}

	public static bool operator !=(ZMMenuOption lhs, ZMMenuOption rhs)
	{
		return !(lhs == rhs);
	}

	int IComparable.CompareTo(object other)
	{
		var option = (ZMMenuOption) other;

		if (option == null) { return 1; }

		if (_index < option._index) return -1;
		if (_index > option._index) return 1;
		else { return 0; }
	}
}
