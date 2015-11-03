using UnityEngine;
using System.Collections;

public class BaseCheckerController : MonoBehaviour
{
	
	void OnTriggerEnter (Collider other)
	{
	}

	void OnTriggerStay (Collider other)
	{
		CheckPlayerLanding ();
	}

	void OnTriggerExit (Collider other)
	{
	}

	public float passCheckTime;
	public float passCheckTimeMax;

	void CheckPlayerLanding ()
	{
		bool passStepCheck = false;
		if (GameController.GetInstance ().Player.inputVec.magnitude < .1f) {
			if (GameController.GetInstance ().Player.PlayerRigidBody.velocity.magnitude < .1f) {
				if (Vector3.Angle (GameController.GetInstance ().Player.transform.up, Vector3.up) < 1f) {
					passStepCheck = true;
				}
			}
		}

		if (passStepCheck) {
			passCheckTime += Time.fixedDeltaTime;
		} else {
			passCheckTime = 0f;
		}

		if (passCheckTime >= passCheckTimeMax) {
			NotificationCenter.DefaultCenter.PostNotification (this, "player_landing_success");
		}

	}
}
