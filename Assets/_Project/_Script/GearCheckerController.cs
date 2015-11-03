using UnityEngine;
using System.Collections;

public class GearCheckerController : MonoBehaviour
{

	public GearController Gear;

	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Wall") || other.CompareTag ("Base")) {
			Gear.GearDown ();
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Wall") || other.CompareTag ("Base")) {
			Gear.GearUp ();
		}
	}
}
