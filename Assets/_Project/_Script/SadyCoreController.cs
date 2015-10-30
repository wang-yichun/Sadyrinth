using UnityEngine;
using System.Collections;

public class SadyCoreController : MonoBehaviour
{

	public SadyController Sady;

	void OnTriggerEnter (Collider other)
	{
		if (other.attachedRigidbody.CompareTag ("Player")) {
			Sady.GetByPlayer (other.attachedRigidbody.gameObject);
		}
	}
}
