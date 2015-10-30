using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	private Rigidbody PlayerRigidBody;

	public Vector2 inputVec;

	public float AutoTorqueRate;

	public EngineController EngineBottom;
	public EngineController EngineLeft;
	public EngineController EngineRight;

	public float MaxFuelValue;
	public float curFuelValue;

	public float CurFuelValue {
		get { return curFuelValue; }
		set { 
			if (value < 0f)
				value = 0f;
			curFuelValue = value;
			NotificationCenter.DefaultCenter.PostNotification (this, "set_fuel_to",
				new Hashtable () { 
					{ "value", curFuelValue }
				}
			);
		}
	}

	public float EngineFuelPowerRate;

	public GameObject CollisionFXPrefab;

	public void InitPlayer ()
	{
		CurFuelValue = MaxFuelValue;
	}

	public void InputVector (Vector2 vec)
	{
		inputVec = vec;
	}

	public static float InputToForceRate = .4f;

	public static Vector3 InputToForce (Vector2 inputVec)
	{
		return new Vector3 (
			inputVec.x * InputToForceRate,
			inputVec.y * InputToForceRate,
			0f
		);
	}

	public static float EngineMaximumForceLimit_Horizontal = 10f;
	public static float EngineMaximumForceLimit_vertical = 30f;

	public static Vector3 EngineMaximumForceLimitFilter (Vector3 inputForce)
	{
		Vector3 outputForce = new Vector3 (inputForce.x, inputForce.y, inputForce.z);

		if (outputForce.y < 0) {
			outputForce.y = 0;
		}

		if (outputForce.y > EngineMaximumForceLimit_vertical) {
			outputForce.y = EngineMaximumForceLimit_vertical;
		}

		if (outputForce.x > EngineMaximumForceLimit_Horizontal) {
			outputForce.x = EngineMaximumForceLimit_Horizontal;
		}

		if (outputForce.x < -EngineMaximumForceLimit_Horizontal) {
			outputForce.x = -EngineMaximumForceLimit_Horizontal;
		}

		return outputForce;
	}

	public void AutoTorque ()
	{
		if (transform.up.x < 0) {
			PlayerRigidBody.AddRelativeTorque (new Vector3 (0f, 0f, -AutoTorqueRate));	
		} else {
			PlayerRigidBody.AddRelativeTorque (new Vector3 (0f, 0f, AutoTorqueRate));	
		}
	}

	public void SetEnginePower (Rigidbody rigidbody, Vector3 force)
	{
		if (CurFuelValue < .01f) {
			CurFuelValue = 0f;

			EngineLeft.SetPower (rigidbody, 0);
			EngineBottom.SetPower (rigidbody, 0);
			EngineRight.SetPower (rigidbody, 0);
		} else {
			
			EngineBottom.SetPower (rigidbody, force.y);

			if (force.x > .1f) {
				EngineLeft.SetPower (rigidbody, force.x);
				EngineRight.SetPower (rigidbody, 0);
			} else if (force.x < .1f) {
				EngineRight.SetPower (rigidbody, -force.x);
				EngineLeft.SetPower (rigidbody, 0);
			} else {
				EngineRight.SetPower (rigidbody, 0);
				EngineLeft.SetPower (rigidbody, 0);
			}

			CurFuelValue -= force.magnitude * EngineFuelPowerRate;
		}

	}

	void Start ()
	{
		PlayerRigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		Vector3 force = InputToForce (inputVec);

		force = EngineMaximumForceLimitFilter (force);

		SetEnginePower (PlayerRigidBody, force);

		GameController.GetInstance ().Pad.SetEngineForce (force);

//		PlayerRigidBody.AddForce (force, ForceMode.Force);

		AutoTorque ();
	}

	void LateUpdate ()
	{
		GameController.GetInstance ().Pad.SetPadRotation (transform.rotation.eulerAngles);
	}

	void OnCollisionEnter (Collision collision)
	{

		Debug.Log ("OnCollisionEnter");

		foreach (ContactPoint contact in collision.contacts) {
//			Debug.DrawRay (contact.point, contact.normal, Color.white);

			GameObject collisionFX = Instantiate (CollisionFXPrefab, contact.point, Quaternion.identity) as GameObject;
		}

		//		if (collision.relativeVelocity.magnitude > 2)
		//			audio.Play(); 

	}
}
