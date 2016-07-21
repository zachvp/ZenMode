using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Core;

[RequireComponent(typeof(Text),
				  typeof(ZMScaleBehavior))]
public class ZMTauntText : MonoBehaviour
{
	private Text _tauntText;
	private ZMScaleBehavior _scaleBehavior;
	private string[] kDeathStrings;

	private const string FILEPATH_TAUNTS = "Taunts/taunts";

	void Awake()
	{
		_tauntText = GetComponent<Text>();
		_scaleBehavior = GetComponent<ZMScaleBehavior>();

		Init();
		HideTauntText();

		ZMPlayerController.OnPlayerDeath += HandlePlayerDeathEvent;
	}

	private void Init()
	{
		kDeathStrings = Utilities.FileIO.ReadAllLinesFromFile(FILEPATH_TAUNTS);
	}

	private void HandlePlayerDeathEvent(ZMPlayerInfoEventArgs args)
	{
		var startScale = new Vector3 (1.5f, 1.5f, 1.5f);
		var endScale = Vector3.one;
		var scaleTime = 0.05f;
		var hideDelay = 0.5f;

		// Show the text and randomize the position and rotation.
		_tauntText.gameObject.SetActive(true);
		_tauntText.text = kDeathStrings [Random.Range (0, kDeathStrings.Length)];
		_tauntText.transform.rotation = Quaternion.Euler (new Vector3 (0.0f, 0.0f, Random.Range (-20, 20)));
		_tauntText.transform.position += new Vector3 (Random.Range (-100, 100), Random.Range (-100, 100), 0.0f);

		// Scale the text so it looks like it's been slapped onto the screen.
		_scaleBehavior.ScaleToTargetOverTime(startScale, endScale, scaleTime);
		Utilities.ExecuteAfterDelay(HideTauntText, hideDelay);
	}

	private void HideTauntText()
	{
		_tauntText.gameObject.SetActive(false);
	}
}
