using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	private Rigidbody rigidBody;
	public Vector2 inputVec;

	public float AutoTorqueRate;

	public EngineController EngineBottom;
	public EngineController EngineLeft;
	public EngineController EngineRight;

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
			rigidBody.AddRelativeTorque (new Vector3 (0f, 0f, -AutoTorqueRate));	
		} else {
			rigidBody.AddRelativeTorque (new Vector3 (0f, 0f, AutoTorqueRate));	
		}
	}

	public void SetEnginePower (Vector3 force)
	{
		EngineBottom.SetPower (force.y);

		if (force.x > .1f) {
			EngineLeft.SetPower (force.x);
			EngineRight.SetPower (0);
		} else if (force.x < .1f) {
			EngineRight.SetPower (-force.x);
			EngineLeft.SetPower (0);
		} else {
			EngineRight.SetPower (0);
			EngineLeft.SetPower (0);
		}
	}

	void Start ()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		Vector3 force = InputToForce (inputVec);

		force = EngineMaximumForceLimitFilter (force);

		SetEnginePower (force);

		rigidBody.AddForce (force, ForceMode.Force);


		AutoTorque ();
	}
}
