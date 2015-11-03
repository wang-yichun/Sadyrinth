using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RootCanvasBase : MonoBehaviour
{

	public void Close ()
	{
		CanvasOutStart ();
		this.gameObject.SetActive (false);
	}

	public void Open ()
	{
		this.gameObject.SetActive (true);
		CanvasInEnd ();
	}

	public virtual void CanvasInEnd ()
	{
		Debug.Log ("CanvasInEnd: " + name);
	}

	public virtual void CanvasOutStart ()
	{
		Debug.Log ("CanvasOutStart: " + name);
	}

	public Button[] Buttons;

	public void SetAllButtonInteractable (bool value)
	{
		if (Buttons != null && Buttons.Length > 0) {
			foreach (Button button in Buttons) {
				button.interactable = value;
			}
		}
	}
}
