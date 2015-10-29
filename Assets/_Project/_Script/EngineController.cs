using UnityEngine;
using System.Collections;

public class EngineController : MonoBehaviour
{

	public ParticleSystem Smoke1;
	public ParticleSystem Smoke2;

	public ParticleSystem MainFire;

	public float PowerToSmokeRate;
	public float PowerToMainFireRate;

	public EngineCheckerHandler EngineChecker;

	public void SetPower (float power)
	{
		if (EngineChecker != null) {
			EngineChecker.EnginePower = power;
		}

		Smoke1.emissionRate = power * PowerToSmokeRate;
		Smoke2.emissionRate = power * PowerToSmokeRate;

		Color color = MainFire.startColor;
		MainFire.startColor = new Color (color.r, color.g, color.b, power * PowerToMainFireRate);
	}
}
