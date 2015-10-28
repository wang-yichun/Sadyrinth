using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
	public Rigidbody rigidBody;
	public Vector2 inputVec;

	public float AutoTorqueRate;

	public void InputVector (Vector2 vec)
	{
		inputVec = vec;
	}

	public static float InputToForceRate = .1f;

	public static Vector3 InputToForce (Vector2 inputVec)
	{
		return new Vector3 (
			inputVec.x * InputToForceRate,
			inputVec.y * InputToForceRate,
			0f
		);
	}

	void Start ()
	{
		rigidBody = GetComponent<Rigidbody> ();
	}

	void FixedUpdate ()
	{
		Vector3 force = InputToForce (inputVec);
		rigidBody.AddForce (force, ForceMode.Force);

		if (transform.up.x < 0) {
			rigidBody.AddRelativeTorque (new Vector3 (0f, 0f, -AutoTorqueRate));	
		} else {
			rigidBody.AddRelativeTorque (new Vector3 (0f, 0f, AutoTorqueRate));	
		}
	}
}
