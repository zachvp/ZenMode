﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ZMPlayer;
using Core;

[RequireComponent(typeof(Text))]
public class ZMMatchOutputDisplay : MonoBehaviour
{
	private Text output;

	private string _victoryMessage;

	private Vector3 outputTextPositionUpOffset = new Vector3(0, 109, 0);

	void Awake()
	{
		output = GetComponent<Text>();
		output.text = "GET READY";

		ZMPauseMenu.OnPlayerPauseGame += ShowPauseOutput;
		ZMTimedCounterStage.GameTimerEndedEvent += ShowGameEndMessage;
		ZMScoreController.OnReachMaxScore += HandleReachMaxScore;

		MatchStateManager.OnMatchResume += ClearText;
		MatchStateManager.OnMatchStart += ShowStartMessage;
	}

	private void HandleReachMaxScore(ZMPlayerInfoEventArgs args)
	{
		ShowGameEndMessage();
	}

	private void ShowPauseOutput(ZMPlayerInfoEventArgs args)
	{
		output.rectTransform.position = outputTextPositionUpOffset;
		output.text = string.Format("P{0} PAUSED", args.info.ID + 1);
	}

	private void ClearText()
	{
		output.text = "";
	}

	private void ShowStartMessage()
	{
		output.text = "Begin!";
		Utilities.ExecuteAfterDelay(ClearText, 1.0f);
	}

	private void ShowGameEndMessage()
	{
		output.rectTransform.position = outputTextPositionUpOffset;
		
		if (ZMCrownManager.LeadingPlayerIndex < 0) { _victoryMessage = "DRAW!"; }
		
		var maxScore = -1.0f;
		
		foreach (ZMScoreController scoreController in ZMPlayerManager.Instance.Scores)
		{
			if (scoreController.TotalScore > maxScore)
			{
				maxScore = scoreController.TotalScore;
				_victoryMessage =  "P" + (scoreController.PlayerInfo.ID + 1) + " WINS!";
            }
        }
        
		output.text = _victoryMessage;
	}

	private void ShowGameEndMessage(ZMPlayerInfo info)
	{
		ShowGameEndMessage();
	}
}
