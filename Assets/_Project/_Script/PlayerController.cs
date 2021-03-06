﻿using UnityEngine;
using System.Collections;
using GameDataEditor;

public class PlayerController : MonoBehaviour
{
	public Rigidbody PlayerRigidBody;

	public Vector2 inputVec;

	public float AutoTorqueRate;

	public EngineController EngineBottom;
	public EngineController EngineLeft;
	public EngineController EngineRight;

	public float ExtraFuelValue;
	public float MaxFuelValue;
	// include stage base fuel
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
	public GameObject CrashCollisionFXPrefab;

	public void InitPlayer ()
	{
		string stage_id = GameController.GetInstance ().LastStartStageID;
		GDEStageData stageData = DataController.GetInstance ().GetStageData (stage_id);

		CurFuelValue = ExtraFuelValue + stageData.base_fuel;

		AudioOn ();
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

			SetThrusterAudioByForce (Vector3.zero);
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

			SetThrusterAudioByForce (force);
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
		GameController game = GameController.GetInstance ();
		if (game.Pad != null) {
			game.Pad.SetPadRotation (transform.rotation.eulerAngles);
		}
	}


	public float CrashTolerateGear;
	public float CrashTolerateSpaceship;
	public float FrictionTolerateGear;
	public float FrictionTolerateSpaceship;

	void OnCollisionEnter (Collision collision)
	{
		DoCollisionCrash (collision);
	}

	void OnCollisionStay (Collision collision)
	{
		DoCollisionFriction (collision);
	}

	void DoCollisionCrash (Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts) {

			if (contact.thisCollider.CompareTag ("Player_Spaceship")) {
				if (collision.impulse.magnitude > CrashTolerateSpaceship) {
					DoUnSafeCrashCollision (contact.point, contact.normal, (collision.impulse.magnitude - CrashTolerateSpaceship) / CrashTolerateSpaceship);
				}
			} else if (contact.thisCollider.CompareTag ("Player_Gear")) {
				if (collision.impulse.magnitude > CrashTolerateGear) {
					DoUnSafeCrashCollision (contact.point, contact.normal, (collision.impulse.magnitude - CrashTolerateGear) / CrashTolerateGear);
				}
			}

		}
	}

	void DoCollisionFriction (Collision collision)
	{
		foreach (ContactPoint contact in collision.contacts) {

			if (contact.thisCollider.CompareTag ("Player_Spaceship")) {
//				
				float f = (collision.relativeVelocity.magnitude * Mathf.Sin (Vector3.Angle (collision.relativeVelocity, -contact.normal)));

				if (f > FrictionTolerateSpaceship) {
					// unsafe-collision
					DoUnSafeFrictionCollision (contact.point, (f - FrictionTolerateSpaceship) / FrictionTolerateSpaceship);
				}
			} else if (contact.thisCollider.CompareTag ("Player_Gear")) {

				float f = (collision.relativeVelocity.magnitude * Mathf.Sin (Vector3.Angle (collision.relativeVelocity, -contact.normal)));

				if (f > FrictionTolerateGear) {
					// unsafe-collision
					DoUnSafeFrictionCollision (contact.point, (f - FrictionTolerateSpaceship) / FrictionTolerateSpaceship);
				}
			}

		}
	}

	Vector3 LastDoUnSafeFrictionCollisionPosition = Vector3.zero;
	float LastDoUnSafeFrictionCollisionTime = 0f;

	void DoUnSafeFrictionCollision (Vector3 position, float value)
	{
		if (LastDoUnSafeFrictionCollisionPosition == Vector3.zero || Vector3.Distance (LastDoUnSafeFrictionCollisionPosition, position) > .1f &&
		    (Time.time - LastDoUnSafeFrictionCollisionTime) > .1f) {

			Debug.Log ("Distance: " + Vector3.Distance (LastDoUnSafeFrictionCollisionPosition, position));
			Debug.Log ("TimeDiff: " + (Time.time - LastDoUnSafeFrictionCollisionTime));

			GameObject collisionFXObject = Instantiate (CollisionFXPrefab, position, Quaternion.identity) as GameObject;
			ParticleSystem collisionFX = collisionFXObject.GetComponent<ParticleSystem> ();
			collisionFX.maxParticles = (int)Mathf.Lerp (1f, 30f, value);

			LastDoUnSafeFrictionCollisionPosition = position;
			LastDoUnSafeFrictionCollisionTime = Time.time;

			if (DataController.GetInstance ().Common.sound) {
				collisionFXObject.GetComponent<AudioSource> ().Play ();
			}
		}
	}

	void DoUnSafeCrashCollision (Vector3 position, Vector3 upVec, float value)
	{
		GameObject collisionFXObject = Instantiate (CrashCollisionFXPrefab, position, Quaternion.identity) as GameObject;
		ParticleSystem collisionFX = collisionFXObject.GetComponent<ParticleSystem> ();
		collisionFX.maxParticles = (int)Mathf.Lerp (1f, 30f, value);
		collisionFX.transform.forward = upVec;

		if (DataController.GetInstance ().Common.sound) {
			collisionFXObject.GetComponent<AudioSource> ().Play ();
		}
	}

	#region Audio

	public AudioSource ThrusterAudio;
	public AudioSource SadyFullTipAudio;

	public void AudioOn ()
	{
		if (DataController.GetInstance ().Common.sound) {
			ThrusterAudio.Play ();
			ThrusterAudio.volume = 0f;
		}
	}

	public void AudioOff ()
	{
		ThrusterAudio.Stop ();
	}

	public void SetThrusterAudioByForce (Vector3 force)
	{
		float volume = force.magnitude / 10f;

		ThrusterAudio.volume = Mathf.Lerp (ThrusterAudio.volume, volume, .1f);
	}

	#endregion
}
