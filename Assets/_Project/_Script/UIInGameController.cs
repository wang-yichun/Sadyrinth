using UnityEngine;
using System.Collections;

public class UIInGameController : MonoBehaviour
{

	public tk2dTextMesh FuelValue;

	// Use this for initialization
	void Awake ()
	{
		NotificationCenter.DefaultCenter.AddObserver (this, "set_fuel_to");
	}

	void OnDestroy ()
	{
		NotificationCenter.DefaultCenter.RemoveObserver (this, "set_fuel_to");
	}

	void set_fuel_to (NotificationCenter.Notification notification)
	{
		FuelValue.text = string.Format ("{0:0}", (float)notification.data ["value"]);
	}
}
