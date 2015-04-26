using UnityEngine;
using System.Collections.Generic;

public class ZMScoreLayoutController : MonoBehaviour {
	private List<RectTransform> _scoreTransforms;
	private int _playerCount;

	// constants
	private const string kScoreGuiTag = "ScoreGui";

	private const float _paddingTop = -20;

	private Vector2 _positionSlot0;
	private Vector2 _positionSlot1;
	private Vector2 _positionSlot2;
	private Vector2 _positionSlot3;

	void Awake() {
		_scoreTransforms = new List<RectTransform>();

		_positionSlot0 = new Vector2(61,  _paddingTop);
		_positionSlot1 = new Vector2(361, _paddingTop);
		_positionSlot2 = new Vector2(661, _paddingTop);
		_positionSlot3 = new Vector2(971, _paddingTop);
	}

	void Start() {
		_playerCount = ZMPlayerManager.NumPlayers;

		for (int i = 0; i < _playerCount; ++i) {
			_scoreTransforms.Add(null);
		}

		foreach (GameObject item in GameObject.FindGameObjectsWithTag("ScoreGui")) {
			int index = (int) item.GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag;

			item.gameObject.SetActive(false);

			if (index < _playerCount)
				_scoreTransforms[index] = item.GetComponent<RectTransform>();
		}

		for (int i = 0; i < _playerCount; ++i) {
			_scoreTransforms[i].gameObject.SetActive(true);
		}

		if (_playerCount <= 2) {
			_scoreTransforms[0].anchoredPosition = _positionSlot0;
			_scoreTransforms[1].anchoredPosition = _positionSlot3;
		} else {

		}
	}
}
