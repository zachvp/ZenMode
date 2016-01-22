using UnityEngine;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;

public class ZMScoreDisplayManager : MonoBehaviour
{
	public Slider[] ScoreSliders { get { return _scoreSliders; } }
	public Text[] ScoreStatuses { get { return _scoreStatuses; } }

	public static ZMScoreDisplayManager Instance
	{
		get
		{
			// TODO: Should be assert.
			if (_instance == null)
			{
				Debug.LogError("ZMScoreManager: no instance exists in the scene.");
			}
			
			return _instance;
		}
	}

	protected static ZMScoreDisplayManager _instance;

	protected float _maxSliderValue;

	private Slider[] _scoreSliders;
	private Text[] _scoreStatuses;

	protected virtual void Awake()
	{
		_scoreSliders = new Slider[Constants.MAX_PLAYERS];
		_scoreStatuses = new Text[Constants.MAX_PLAYERS];

		// TODO: Should be assert.
		if (_instance != null)
		{
			Debug.LogError("ZMScoreManager: More than one instance exists in the scene.");
		}
		
		_instance = this;

		_maxSliderValue = ZMScoreController.MAX_SCORE;

		ZMScoreController.MinScoreReached += EliminateScore;
		ZMScoreController.OnUpdateScore += UpdateScore;

		ConfigureScoreSliders();
		ConfigureScoreStatuses();
	}

	void OnDestroy()
	{
		_instance = null;
	}

	protected void ConfigureScoreSliders()
	{
		var sliderObjects = GameObject.FindGameObjectsWithTag(Tags.kScoreGui);
		
		for (int i = 0; i < sliderObjects.Length; ++i)
		{
			var info = sliderObjects[i].GetComponent<ZMPlayerInfo>();
			
			_scoreSliders[info.ID] = sliderObjects[i].GetComponent<Slider>();
			_scoreSliders[info.ID].handleRect = null;
			_scoreSliders[info.ID].maxValue = _maxSliderValue;
			_scoreSliders[info.ID].value = 0.0f;
		}
	}
	
	protected void ConfigureScoreStatuses()
	{
		var statusObjects = GameObject.FindGameObjectsWithTag(Tags.kScoreStatus);
		
		for (int i = 0; i < statusObjects.Length; ++i)
		{
			var info = statusObjects[i].GetComponent<ZMPlayerInfo>();
			
			_scoreStatuses[info.ID] = statusObjects[i].GetComponent<Text>();
			_scoreStatuses[info.ID].text = "";
		}
	}

	protected void ActivateScoreBars()
	{
		for (int i = 0; i < _scoreSliders.Length; ++i)
		{
			_scoreSliders[i].gameObject.SetActive(true);
		}
	}

	protected void ActivateScoreBar(ZMPlayerInfo info)
	{
		ActivateScoreBar(info.ID);
	}

	protected void ActivateScoreBar(int id)
	{
		_scoreSliders[id].gameObject.SetActive(true);
	}

	protected void DeactivateScoreBars()
	{
		for (int i = 0; i < _scoreSliders.Length; ++i)
		{
			_scoreSliders[i].gameObject.SetActive(false);
		}
	}
	
	protected void DeactivateScoreBar(ZMPlayerInfo info)
	{
		DeactivateScoreBar(info.ID);
	}

	protected void DeactivateScoreBar(int id)
	{
		_scoreSliders[id].gameObject.SetActive(false);
	}
	
	protected void EliminateScore(ZMScoreController controller)
	{
		_scoreStatuses[controller.PlayerInfo.ID].text = "ELIMINATED!";
	}

	protected void UpdateScore(ZMPlayerInfo info, float amount)
	{		
		_scoreSliders[info.ID].value = amount;
	}
}
