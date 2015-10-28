using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GearController : MonoBehaviour {

	public float ActionTime;
	public Vector3 DownPosition;
	public Vector3 UpPosition;

	// 放起落架
	public void GearDown() {
		transform.DOLocalMove (DownPosition, ActionTime);
	}

	// 收起落架
	public void GearUp() {
		transform.DOLocalMove (UpPosition, ActionTime);
	}
}
