using UnityEngine;
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
}
