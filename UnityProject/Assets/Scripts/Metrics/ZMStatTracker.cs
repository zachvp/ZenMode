using System;
using UnityEngine;
using ZMPlayer;
using ZMConfiguration;

public static class ZMStatTracker
{
	// stats
	public static Stat Kills { get { return _kills; } }
	public static Stat GrassCuts { get { return _grassCuts; } }

	private static Stat _kills = new Stat(Constants.MAX_PLAYERS);
	private static Stat _grassCuts = new Stat(Constants.MAX_PLAYERS);

	// stat inner class
	public class Stat
	{
		private int[] _array;

		public Stat(int maxSize) { _array = new int[maxSize]; }

		public void Add(ZMPlayerInfo playerInfo) { _array[playerInfo.ID] += 1; }
		public int GetStat(ZMPlayerInfo playerInfo) { return _array[playerInfo.ID]; }

		public int[] GetMax()
		{
			int[] max = { 0, 0 };
			
			for (int i = 0; i < _array.Length; ++i)
			{
				if (_array[i] > max[1]) {
					max[0] = i;
					max[1] = _array[i];
				}
			}
			
			return max;
		}
	}
}