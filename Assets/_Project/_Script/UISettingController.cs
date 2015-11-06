using UnityEngine;
using System.Collections;

public class UISettingController : RootCanvasBase
{

	public override void CanvasInEnd ()
	{
		base.CanvasInEnd ();
		Refresh ();
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();
	}

	public tk2dTextMesh MusicLabel;
	public tk2dTextMesh SoundLabel;

	public void Refresh ()
	{
		SetMusicLabel (DataController.GetInstance ().Common.music);
		SetSoundLabel (DataController.GetInstance ().Common.sound);
	}

	void SetMusicLabel (bool value)
	{
		MusicLabel.text = value ? "Music On" : "Music Off";
	}

	void SetSoundLabel (bool value)
	{
		SoundLabel.text = value ? "Sound On" : "Sound Off";
	}
}
