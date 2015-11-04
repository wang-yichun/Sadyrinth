using UnityEngine;
using System.Collections;

public class NoFuelCheckerController : MonoBehaviour
{

	void Start() {
		NotificationCenter.DefaultCenter.AddObserver (this, "start_game_reset");
	}

	void OnDestroy ()
	{
		NotificationCenter.DefaultCenter.RemoveObserver (this, "start_game_reset");
	}

	void start_game_reset (NotificationCenter.Notification notification)
	{
		Debug.Log ("start_game_reset @ NoFuelCheckerController");
		ResetPassCheck ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		CheckPlayerNoFuel ();
	}

	public float passCheckTime_NoFuel;
	public float passCheckTimeMax_NoFuel;

	public void ResetPassCheck ()
	{
		passCheckTime_NoFuel = 0f;
	}

	void CheckPlayerNoFuel ()
	{
		
		bool passStepCheck = false;

		if (GameController.GetInstance ().Player.CurFuelValue < .1f) {
			if (GameController.GetInstance ().Player.PlayerRigidBody.velocity.magnitude < .1f) {
				passStepCheck = true;
			}
		}


		if (passStepCheck) {
			passCheckTime_NoFuel += Time.fixedDeltaTime;
		} else {
			passCheckTime_NoFuel = 0f;
		}

		if (passCheckTime_NoFuel >= passCheckTimeMax_NoFuel) {
			NotificationCenter.DefaultCenter.PostNotification (this, "player_no_fuel");
			passCheckTime_NoFuel = 0f;
		}
	}
}
