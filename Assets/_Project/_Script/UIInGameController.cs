using UnityEngine;
using System.Collections;

public class UIInGameController : RootCanvasBase
{

	public tk2dTextMesh StageIDValue;
	public tk2dTextMesh FuelValue;
	public tk2dTextMesh SadyValue;

	public override void CanvasInEnd ()
	{
		base.CanvasInEnd ();
		AddObservers ();
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();
		RemoveObservers ();
	}

	// Use this for initialization
	void AddObservers ()
	{
		NotificationCenter.DefaultCenter.AddObserver (this, "set_stage_id_to");
		NotificationCenter.DefaultCenter.AddObserver (this, "set_fuel_to");
		NotificationCenter.DefaultCenter.AddObserver (this, "set_sady_to");
	}

	void RemoveObservers ()
	{
		NotificationCenter.DefaultCenter.RemoveObserver (this, "set_stage_id_to");
		NotificationCenter.DefaultCenter.RemoveObserver (this, "set_fuel_to");
		NotificationCenter.DefaultCenter.RemoveObserver (this, "set_sady_to");
	}

	void set_stage_id_to (NotificationCenter.Notification notification)
	{
		StageIDValue.text = string.Format ("{0:0}", (string)notification.data ["value"]);
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
