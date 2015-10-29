using UnityEngine;
using System.Collections;

using DG.Tweening;

public class ControlPadController : MonoBehaviour
{

	public Transform Pad;
	public Transform Pointer;

	public void Open ()
	{
		Pointer.GetComponent<SpriteRenderer> ().material.DOFade (1f, .2f);
		Pad.GetComponent<SpriteRenderer> ().material.DOFade (1f, .4f);
	}

	public void Close ()
	{
		Pointer.GetComponent<SpriteRenderer> ().material.DOFade (0f, .2f);
		Pad.GetComponent<SpriteRenderer> ().material.DOFade (0f, .4f);
	}

	public void SetPadPosition (Vector3 position, bool isTween = false)
	{
		if (isTween) {
			transform.DOMove (position, .2f).SetEase (Ease.InOutSine);
		} else {
			transform.position = position;
		}
	}

	public void SetEngineForce (Vector3 force)
	{

		Vector3 forceFix = force / PlayerController.EngineMaximumForceLimit_Horizontal * PlayerController.InputToForceRate;


		Pointer.transform.up = transform.rotation * force;

		Pointer.transform.localScale = new Vector3 (1f, forceFix.magnitude, 1f);
	}

	public void SetPadRotation (Vector3 eular)
	{
		transform.rotation = Quaternion.Euler (eular);
	}
}
