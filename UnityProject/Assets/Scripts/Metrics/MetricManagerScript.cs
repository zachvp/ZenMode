using UnityEngine;
using System.Collections;
using System.IO;
using ZMPlayer;

public class MetricManagerScript : MonoBehaviour
{
	string createText = "";

	void Awake()
	{
		ZMMetricsCollector.MetricsAddPositionEvent += HandleAddPositionEvent;

		AcceptPlayerEvents();
	}
	
	//When the game quits we'll actually write the file.
	void OnApplicationQuit()
	{
		string time = System.DateTime.UtcNow.ToString ();//string dateTime = System.DateTime.Now.ToString (); //Get the time to tack on to the file name
		time = time.Replace ("/", "-"); //Replace slashes with dashes, because Unity thinks they are directories..
		time = time.Replace (":", "-");
		string reportFile = "Metrics/ZenMode_Metrics_" + time + ".txt"; 
		File.WriteAllText (reportFile, createText);
		//In Editor, this will show up in the project folder root (with Library, Assets, etc.)
		//In Standalone, this will show up in the same directory as your executable
	}

	private void AcceptPlayerEvents()
	{
		ZMPlayerManager.Instance.OnPlayerDeath += HandlePlayerDeathEvent;
	}

	private void HandleAddPositionEvent(int player, Vector3 position)
	{
		createText += "Player " + player.ToString() + " Position: " + position.ToString() + "\n";
	}

	private void HandlePlayerDeathEvent(ZMPlayerInfo info)
	{
		createText += "Player " + info.ID.ToString() + " Death: " + info.transform.position.ToString() + "\n";
	}
}
