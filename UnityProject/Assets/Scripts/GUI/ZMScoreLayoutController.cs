using UnityEngine;
using System.Collections.Generic;

public class ZMScoreLayoutController : MonoBehaviour {
	private List<RectTransform> _scoreTransforms;
	private List<RectTransform> _scoreStatusTransforms;
	private int _playerCount;

	// constants
	private const string kScoreGuiTag = "ScoreGui";

	private const float _paddingTop = -20;

	private Vector2 _positionSlot0;
    //private Vector2 _positionSlot1;
	private Vector2 _positionSlot2;
	//private Vector2 _positionSlot3;

	void Awake() {
		_scoreTransforms = new List<RectTransform>();
		_scoreStatusTransforms = new List<RectTransform>();

		_positionSlot0 = new Vector2(32,  _paddingTop);
		//_positionSlot1 = new Vector2(314, _paddingTop);
	    _positionSlot2 = new Vector2(716, _paddingTop);
		//_positionSlot3 = new Vector2(998, _paddingTop);
	}

	void Start() {
		_playerCount = ZMPlayerManager.PlayerCount;

		for (int i = 0; i < _playerCount; ++i) {
			_scoreTransforms.Add(null);
			_scoreStatusTransforms.Add(null);
		}

		foreach (GameObject item in GameObject.FindGameObjectsWithTag("ScoreGui")) {
			int index = (int) item.GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag;

			item.gameObject.SetActive(false);

			if (index < _playerCount)
				_scoreTransforms[index] = item.GetComponent<RectTransform>();
		}

		foreach (GameObject item in GameObject.FindGameObjectsWithTag("ScoreStatus")) {
			int index = (int) item.GetComponent<ZMPlayer.ZMPlayerInfo>().playerTag;

			if (index < _playerCount)
				_scoreStatusTransforms[index] = item.GetComponent<RectTransform>();
		}

		for (int i = 0; i < _playerCount; ++i) {
			_scoreTransforms[i].gameObject.SetActive(true);
		}

		if (_playerCount <= 2) {
			_scoreTransforms[0].anchoredPosition = _positionSlot0;
			_scoreTransforms[1].anchoredPosition = _positionSlot2;

			_scoreTransforms[0].localScale = new Vector3 (5.0f, 3.0f, 1.0f);
			_scoreTransforms[1].localScale = new Vector3 (5.0f, 3.0f, 1.0f);
			_scoreTransforms[1].anchoredPosition = new Vector2 (742, _paddingTop);

			_scoreStatusTransforms[0].anchoredPosition = _positionSlot0 + new Vector2(_scoreStatusTransforms[0].rect.width * 1.5f, -_scoreStatusTransforms[0].rect.height * 0.7f);
		}
	}
}
