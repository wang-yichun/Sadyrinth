using UnityEngine;
using System.Collections;

public class EnvironmentController : MonoBehaviour
{

	// Use this for initialization
	void Start ()
	{
		ReCenterTerrian ();
	}

	void ReCenterTerrian ()
	{
		Ferr2D_Path[] list = GetComponentsInChildren<Ferr2D_Path> ();
		foreach (Ferr2D_Path item in list) {
			item.ReCenter ();
		}
	}
}
