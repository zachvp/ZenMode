using UnityEngine;
using System.Collections.Generic;
using ZMConfiguration;
using ZMPlayer;

public class ZMScoreLayoutController : MonoBehaviour
{
	private RectTransform[] _scoreTransforms;
	private int _playerCount;

	// constants
	private const string kScoreGuiTag = "ScoreGui";

	private const float _paddingTop = -20;

	private Vector2 _positionSlot0;
	private Vector2 _positionSlot2;

	void Awake()
	{
		_playerCount = Settings.MatchPlayerCount.value;

		_scoreTransforms = new RectTransform[_playerCount];

		_positionSlot0 = new Vector2(32,  _paddingTop);
	    _positionSlot2 = new Vector2(716, _paddingTop);
	}

	void Start()
	{
		foreach (GameObject item in GameObject.FindGameObjectsWithTag(Tags.kScoreGui))
		{
			int index = item.GetComponent<ZMPlayer.ZMPlayerInfo>().ID;

			item.gameObject.SetActive(false);

			if (index < _playerCount)
				_scoreTransforms[index] = item.GetComponent<RectTransform>();
		}

		for (int i = 0; i < _playerCount; ++i)
		{
			_scoreTransforms[i].gameObject.SetActive(true);
		}

		if (_playerCount == 1)
		{
			_scoreTransforms[0].anchoredPosition = _positionSlot0;
			_scoreTransforms[0].localScale = new Vector3 (5.0f, 3.0f, 1.0f);
		}
		else if (_playerCount == 2)
		{
			_scoreTransforms[0].anchoredPosition = _positionSlot0;
			_scoreTransforms[1].anchoredPosition = _positionSlot2;

			_scoreTransforms[0].localScale = new Vector3 (5.0f, 3.0f, 1.0f);
			_scoreTransforms[1].localScale = new Vector3 (5.0f, 3.0f, 1.0f);
			_scoreTransforms[1].anchoredPosition = new Vector2 (742, _paddingTop);
		}
	}
}
