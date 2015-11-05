﻿using UnityEngine;
using System.Collections;

public class UILoseController : RootCanvasBase
{

	public PauseStatisticsLike StatisticsLike;

	public override void CanvasInEnd ()
	{
		base.CanvasInEnd ();

		GetComponent<AudioSource> ().Play ();
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();
	}
}
