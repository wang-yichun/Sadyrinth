using UnityEngine;
using System.Collections;

public class UIMainMenuController : RootCanvasBase {

	public override void CanvasInEnd ()
	{
		base.CanvasInEnd ();

		GameController.GetInstance ().PlayMusic (GameController.GetInstance ().MainThemeMusic);
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();
	}
}
