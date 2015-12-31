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

	private static ZMScoreDisplayManager _instance;

	private Slider[] _scoreSliders;
	private Text[] _scoreStatuses;

	void Awake()
	{
		_scoreSliders = new Slider[Constants.MAX_PLAYERS];
		_scoreStatuses = new Text[Constants.MAX_PLAYERS];

		// TODO: Should be assert.
		if (_instance != null)
		{
			Debug.LogError("ZMScoreManager: More than one instance exists in the scene.");
		}
		
		_instance = this;

		var sliderObjects = GameObject.FindGameObjectsWithTag(Tags.kScoreGui);
		var statusObjects = GameObject.FindGameObjectsWithTag(Tags.kScoreStatus);
		
		for (int i = 0; i < sliderObjects.Length; ++i)
		{
			var info = sliderObjects[i].GetComponent<ZMPlayerInfo>();
			
			_scoreSliders[info.ID] = sliderObjects[i].GetComponent<Slider>();
			_scoreSliders[info.ID].handleRect = null;
			_scoreSliders[info.ID].maxValue = ZMScoreController.MAX_SCORE;
			_scoreSliders[info.ID].value = Settings.MatchPlayerCount.value > 2 ? ZMScoreController.MAX_SCORE / 2f : ZMScoreController.MAX_SCORE / Settings.MatchPlayerCount.value;
		}
		
		for (int i = 0; i < statusObjects.Length; ++i)
		{
			var info = statusObjects[i].GetComponent<ZMPlayerInfo>();
			
			_scoreStatuses[info.ID] = statusObjects[i].GetComponent<Text>();
			_scoreStatuses[info.ID].text = "";
		}

		ZMScoreController.MinScoreReached += EliminateScore;
		ZMScoreController.UpdateScoreEvent += UpdateScore;
	}
	
	void OnDestroy()
	{
		_instance = null;
	}

	private void EliminateScore(ZMScoreController controller)
	{
		_scoreStatuses[controller.PlayerInfo.ID].text = "ELIMINATED!";
	}

	private void UpdateScore(ZMScoreController controller)
	{		
		_scoreSliders[controller.PlayerInfo.ID].value = controller.TotalScore;
	}
}
