using System;
using UnityEngine;
using ZMPlayer;
using ZMConfiguration;

public sealed class ZMStatTracker {
	private static readonly ZMStatTracker _instance = new ZMStatTracker(); public static ZMStatTracker Instance { get { return _instance; } }

	// stats
	private Stat _kills; public Stat Kills { get { return _kills; } }
	private Stat _grassCuts; public Stat GrassCuts { get { return _grassCuts; } }

	// private constructor for singleton
	private ZMStatTracker() {
		int statSize = Constants.MAX_PLAYERS;

		_kills = new Stat(statSize);
		_grassCuts = new Stat(statSize);
	}

	// stat inner class
	public class Stat {
		private int[] _array;

		public Stat(int maxSize) { _array = new int[maxSize]; }

		public void Add(ZMPlayerInfo playerInfo) { _array[(int) playerInfo.playerTag] += 1; }
		public int GetStat(ZMPlayerInfo playerInfo) { return _array[(int) playerInfo.playerTag]; }
		public int[] GetMax() {
			int[] max = { 0, 0 };
			
			for (int i = 0; i < _array.Length; ++i) {
				if (_array[i] > max[1]) {
					max[0] = i;
					max[1] = _array[i];
				}
			}
			
			return max;
		}
	}
}