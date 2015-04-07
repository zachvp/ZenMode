using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZMWarpVolume : MonoBehaviour {
	public ZMWarpVolume sibling;

	private float _siblingAxisThreshold;
	private enum PositionHorizontalRelativeToSibling { NEUTRAL, LEFT, RIGHT };
	private enum PositionVerticalRelativeToSibling { NEUTRAL, ABOVE, BELOW };

	private PositionHorizontalRelativeToSibling _relativeHorizontalPosition;
	private PositionVerticalRelativeToSibling _relativeVerticalPosition;

	void Awake() {
		_siblingAxisThreshold = 64.0f;
		_relativeHorizontalPosition = PositionHorizontalRelativeToSibling.NEUTRAL;
		_relativeVerticalPosition = PositionVerticalRelativeToSibling.NEUTRAL;
	}

	void Start() {
		float selfPos, siblingPos;
		selfPos = transform.position.x;
		siblingPos = sibling.transform.position.x;

		if (Mathf.Abs(selfPos - siblingPos) > _siblingAxisThreshold) {
			_relativeHorizontalPosition = selfPos < siblingPos ? PositionHorizontalRelativeToSibling.LEFT 
															   : PositionHorizontalRelativeToSibling.RIGHT;
		}


		selfPos = transform.position.y;
		siblingPos = sibling.transform.position.y;
		if (Mathf.Abs(selfPos - siblingPos) > _siblingAxisThreshold) {
			_relativeVerticalPosition = selfPos < siblingPos ? PositionVerticalRelativeToSibling.BELOW 
															   : PositionVerticalRelativeToSibling.ABOVE;
		}
	}

	// public methods
	public void Warp(GameObject warpObject) {
		Vector3 spawnPosition = sibling.transform.position;
		float offset;

		if (_relativeHorizontalPosition != PositionHorizontalRelativeToSibling.NEUTRAL) {
			offset = _siblingAxisThreshold / 2.0f + 1.0f;
			offset *= _relativeHorizontalPosition == PositionHorizontalRelativeToSibling.LEFT ? -1 : 1;
			spawnPosition.x += offset;
			spawnPosition.y = warpObject.transform.position.y;
		} else if (_relativeVerticalPosition != PositionVerticalRelativeToSibling.NEUTRAL) {
			offset = _siblingAxisThreshold + 1.0f;
			offset *= _relativeVerticalPosition == PositionVerticalRelativeToSibling.BELOW ? -1 : 1;
			spawnPosition.y += offset;
			spawnPosition.x = warpObject.transform.position.x;
		}

		warpObject.transform.position = spawnPosition;
	}
}
