using UnityEngine;
using System;
using System.Collections;

namespace ZMPlayer
{
	public class ZMPlayerInfo : MonoBehaviour, IComparable
	{
		[SerializeField] private int id;

		public int ID { get { return id; } set { id = value; } }

		public Color standardColor { get; set; } // TODO: Have this be set in constructor when this is not a MonoBehaviour.
		public Color lightColor { get; set; }

		public override bool Equals(System.Object other)
		{
			var otherInfo = other as ZMPlayerInfo;

			if (otherInfo == null) { return false; }

			return id == otherInfo.id;
		}

		public override int GetHashCode()
		{
			return id.GetHashCode();
		}

		public static bool operator ==(ZMPlayerInfo lhs, ZMPlayerInfo rhs)
		{
			// Automatically equal if same reference (or both null).
			if (System.Object.ReferenceEquals(lhs, rhs)) { return true; }

			// If one is null, but not both, return false.
			if ((object) lhs == null || (object) rhs == null) { return false; }

			return lhs.id == rhs.id;
		}

		public static bool operator !=(ZMPlayerInfo lhs, ZMPlayerInfo rhs)
		{
			return !(lhs == rhs);
		}

		int IComparable.CompareTo(object other)
		{
			var otherInfo = (ZMPlayerInfo) other;
			int result = 0;

			if (otherInfo == null) { result = 1; }
			else if (id < otherInfo.id) { result = -1; }
			else if (id > otherInfo.id) { result = 1; }

			return result;
		}
	}
}