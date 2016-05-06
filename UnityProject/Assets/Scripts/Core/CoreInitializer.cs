using UnityEngine;
using Core;

public class CoreInitializer : MonoBehaviour
{
	void Awake()
	{
		Utilities.Init(this);
	}
}
