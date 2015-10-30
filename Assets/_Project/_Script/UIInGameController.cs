using UnityEngine;
using System.Collections;

public class UIInGameController : MonoBehaviour
{

	public tk2dTextMesh FuelValue;
	public tk2dTextMesh SadyValue;

	// Use this for initialization
	void Awake ()
	{
		NotificationCenter.DefaultCenter.AddObserver (this, "set_fuel_to");
		NotificationCenter.DefaultCenter.AddObserver (this, "set_sady_to");
	}

	void OnDestroy ()
	{
		NotificationCenter.DefaultCenter.RemoveObserver (this, "set_sady_to");
	}

	void set_fuel_to (NotificationCenter.Notification notification)
	{
		FuelValue.text = string.Format ("{0:0}", (float)notification.data ["value"]);
	}

	void set_sady_to (NotificationCenter.Notification notification)
	{
		SadyValue.text = string.Format ("{0:0}/{1:0}", (int)notification.data ["value"], (int)notification.data ["total_value"]);
	}
}
