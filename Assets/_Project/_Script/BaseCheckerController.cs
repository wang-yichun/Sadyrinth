using UnityEngine;
using System.Collections;

public class BaseCheckerController : MonoBehaviour
{

	void Start ()
	{
		NotificationCenter.DefaultCenter.AddObserver (this, "start_game_reset");
	}

	void OnDestroy ()
	{
		NotificationCenter.DefaultCenter.RemoveObserver (this, "start_game_reset");
	}

	void start_game_reset (NotificationCenter.Notification notification)
	{
		Debug.Log ("start_game_reset @ BaseCheckerController");
		ResetPassCheck ();
	}

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

	public float passCheckTime_Landing;
	public float passCheckTimeMax_Landing;

	public void ResetPassCheck ()
	{
		passCheckTime_Landing = 0f;
	}

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
			passCheckTime_Landing += Time.fixedDeltaTime;
		} else {
			passCheckTime_Landing = 0f;
		}

		if (passCheckTime_Landing >= passCheckTimeMax_Landing) {
			passCheckTime_Landing = 0f;
			NotificationCenter.DefaultCenter.PostNotification (this, "player_landing_success");
		}
	}

}
