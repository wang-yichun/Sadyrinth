using UnityEngine;
using System.Collections;

public class UIWinController : RootCanvasBase
{

	public PauseStatisticsLike StatisticsLike;

	public override void CanvasInEnd ()
	{
		base.CanvasInEnd ();

		if (DataController.GetInstance ().Common.sound) {
			GetComponent<AudioSource> ().Play ();
		}
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();
	}
}
