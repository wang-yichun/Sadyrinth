using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CanvasOKCancelBase : RootCanvasBase
{
	public tk2dTextMesh InfoLabel;
	public InputField SecretCodeInputField;

	public CanvasOKCancelInfo Info;

	public Button OKButton;

	public override void Open ()
	{
		Debug.LogWarning ("需要使用带参数的Open");
		base.Open ();
	}

	public void Open (CanvasOKCancelInfo info)
	{
		Info = info;
		Refresh ();
		SecretCodeInputField.text = "";

		base.Open ();
	}

	public override void Close ()
	{
		base.Close ();
	}

	public void Refresh ()
	{
		InfoLabel.text = Info.ContentInfo;
		AutoSetOKButtonInteractable ();
	}

	public void AutoSetOKButtonInteractable ()
	{
		if (IsInputSecretCorrect ()) {
			OKButton.interactable = true;
		} else {
			OKButton.interactable = false;
		}
	}

	public bool IsInputSecretCorrect ()
	{
		return Info.CorrectSecretString == null || SecretCodeInputField.text == Info.CorrectSecretString;
	}

	public void OnValueChange (string value)
	{
		AutoSetOKButtonInteractable ();
	}

	public void EndEdit (string value)
	{
		if (Info.CorrectSecretString == null)
			return;
		
		if (IsInputSecretCorrect ()) {
			SummitResult ();
		}
	}

	public void CancelButton_OnClicked ()
	{
		CancelSummit ();
	}

	public void OKButton_OnClicked ()
	{
		SummitResult ();
	}

	public void CancelSummit ()
	{
		Info.On_Cancel.Invoke ();
		this.Close ();
	}

	public void SummitResult ()
	{
		Info.On_OK.Invoke (SecretCodeInputField.text);
		this.Close ();
	}

	public override void CanvasInEnd ()
	{
		base.CanvasInEnd ();

		if (Info.parentCanvas != null)
			Info.parentCanvas.SetAllButtonInteractable (false);
	}

	public override void CanvasOutStart ()
	{
		base.CanvasOutStart ();

		if (Info.parentCanvas != null)
			Info.parentCanvas.SetAllButtonInteractable (true);
	}
}

public class CanvasOKCancelInfo
{
	public string ContentInfo;

	public string CorrectSecretString;

	public Action<string> On_OK;
	public Action On_Cancel;

	public RootCanvasBase parentCanvas;
}