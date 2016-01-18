using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using ZMConfiguration;
using ZMPlayer;
using Core;

public class ZMScoreController : ZMPlayerItem
{
	// Events
	public static EventHandler<ZMPlayerInfo> OnMaxScoreReached;
	public static EventHandler<ZMScoreController> MinScoreReached;
	public static EventHandler<ZMScoreController> OnStopScore;
	public static EventHandler<ZMPlayerInfo, float> OnUpdateScore;

	public const float MAX_SCORE = 1000.0f;	// TODO: Move to constants class and refactor.

	protected const float SCORE_RATE = 0.5f;

	// References
	private List<ZMScoreController> _allScoreControllers;
	
	// Constants
	private const string kUpdateScoreInvokeWrapperMethodName  = "UpdateScoreInvokeWrapper";
	private const string kScoreFormat						  = "0.0";

	//private string _playerName;
	protected float _totalScore;   public float TotalScore { get { return _totalScore; } set { _totalScore = value; } }
	
	protected override void Awake()
	{
		_playerController = GetComponent<ZMPlayerController>();

		base.Awake();

		_totalScore = 0;

		_allScoreControllers = new List<ZMScoreController>();
	}

	protected virtual void OnDestroy()
	{
		OnMaxScoreReached   = null;
		OnUpdateScore   	= null;
		OnStopScore	   		= null;
	}

	public override void ConfigureItemWithID(int id)
	{
		base.ConfigureItemWithID(id);
		
		GameObject[] scoreObjects = GameObject.FindGameObjectsWithTag(Tags.kPlayerTag);
		
		foreach (GameObject scoreObject in scoreObjects)
		{
			_allScoreControllers.Add(scoreObject.GetComponent<ZMScoreController>());
		}
	}
	
	public void SetScore(float newScore)
	{
		_totalScore = Mathf.Max(newScore, 0);
		_totalScore = Mathf.Min(newScore, MAX_SCORE);
		
		Notifier.SendEventNotification(OnUpdateScore, _playerInfo, _totalScore);
	}

	// utility methods
	public void AddToScore(float amount)
	{
		SetScore(_totalScore + amount);
	}
}