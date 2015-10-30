using UnityEngine;
using System.Collections;
using DG.Tweening;

public class SadyController : MonoBehaviour
{

	public Transform SadyDots;
	public Transform SadyCore;

	// Use this for initialization
	void Start ()
	{
		SadyDots.rotation = Quaternion.Euler (0f, 0f, UnityEngine.Random.Range (0f, 360f));

//		SadyDots.localScale = new Vector3 (UnityEngine.Random.Range (.8f, 1.2f), 1f, 1f);

		SadyDots.DORotate (new Vector3 (0f, 0f, 120f), 1f).SetRelative ().SetLoops (-1, LoopType.Incremental).SetEase (Ease.Linear);
		SadyDots.DOScaleX (1.2f, .5f).SetLoops (-1, LoopType.Yoyo).SetEase (Ease.Linear);

		SadyCore.DORotate (new Vector3 (0f, 0f, -60f), 1f).SetRelative ().SetLoops (-1, LoopType.Incremental).SetEase (Ease.Linear);
	}

	public void GetByPlayer (GameObject player)
	{
		gameObject.SetActive (false);

		// todo: FX
	}
}
