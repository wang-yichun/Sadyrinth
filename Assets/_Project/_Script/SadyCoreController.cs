using UnityEngine;
using System.Collections;

public class SadyCoreController : MonoBehaviour
{

	public SadyController Sady;

	void OnTriggerEnter (Collider other)
	{
		if (other.attachedRigidbody.CompareTag ("Player")) {
			Sady.GetByPlayer (other.attachedRigidbody.gameObject);

			if (GameController.GetInstance ().RemainSadyCount == 0) {
				if (DataController.GetInstance ().Common.sound) {
					GameController.GetInstance ().Player.SadyFullTipAudio.PlayDelayed (1f);
				}
			}
		}
	}
}
