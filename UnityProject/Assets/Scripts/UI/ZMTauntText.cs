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

	void Awake()
	{
		_tauntText = GetComponent<Text>();
		_scaleBehavior = GetComponent<ZMScaleBehavior>();

		InitDeathStrings();
		HideTauntText();

		ZMPlayerController.PlayerDeathEvent += HandlePlayerDeathEvent;
	}

	private void InitDeathStrings()
	{
		kDeathStrings = new string[39];

		kDeathStrings[0] = "OOOAHH";
		kDeathStrings[1] = "WHOOOP";
		kDeathStrings[2] = "AYYYEEH";
		kDeathStrings[3] = "HADOOOP";
		kDeathStrings[4] = "WHUAAAH";
		kDeathStrings[5] = "BLARHGH";
		kDeathStrings[6] = "OUCH";
		kDeathStrings[7] = "DUN GOOFD";
		kDeathStrings[8] = "REKT";
		kDeathStrings[9] = "PWNED";
		kDeathStrings[10] = "SPLAT";
		kDeathStrings[11] = "SPLUUSH";
		kDeathStrings[12] = "ASDF";
		kDeathStrings[13] = "WHAAUH";
		kDeathStrings[14] = "AUUGH";
		kDeathStrings[15] = "WAOOOH";
		kDeathStrings[16] = "DERP";
		kDeathStrings[17] = "DISGRACE";
		kDeathStrings[18] = "DISHONOR";
		kDeathStrings[19] = "HUUUAP";
		kDeathStrings[20] = "PUUUAH";
		kDeathStrings[21] = "AYUUSH";
		kDeathStrings[22] = "WYAAAH";
		kDeathStrings[23] = "KWAAAH";
		kDeathStrings[24] = "HUZZAH";
		kDeathStrings[25] = "#WINNING";
		kDeathStrings[26] = "NOOB";
		kDeathStrings[27] = "ELEGANT";
		kDeathStrings[28] = "SWIFT";
		kDeathStrings[29] = "WAHH";
		kDeathStrings[30] = "OOOOOOHH";
		kDeathStrings[31] = "POOOOW";
		kDeathStrings[32] = "YAAAAS";
		kDeathStrings[33] = "SWOOOP";
		kDeathStrings[34] = "LOLWUT";
		kDeathStrings[35] = "SMOOTH";
		kDeathStrings[36] = "YUUUUS";
		kDeathStrings[37] = "YEESSS";
		kDeathStrings[38] = "NOICE";
	}

	private void HandlePlayerDeathEvent(ZMPlayer.ZMPlayerInfo info)
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
