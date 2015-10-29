using UnityEngine;
using System.Collections;

public class EngineCheckerHandler : MonoBehaviour
{

	public GameObject PuffGroundParticleSystemPrefab;

	private ParticleSystem PuffGroundParticleSystem;

	public float EnginePower;

	void OnTriggerEnter (Collider other)
	{
		if (other.CompareTag ("Wall")) {
			PuffGroundFX_Start ();
		}
	}

	void OnTriggerStay (Collider other)
	{
		if (other.CompareTag ("Wall")) {
			PuffGroundFX_Step ();
		}
	}

	void OnTriggerExit (Collider other)
	{
		if (other.CompareTag ("Wall")) {
			PuffGroundFX_End ();
		}
	}

	void PuffGroundFX_Start ()
	{
		// I.找到释放的位置
		RaycastHit raycastHit;
		if (CalcPuffGroundHit (out raycastHit)) { 
			// II.生成
			PuffGroundParticleSystem = Instantiate<GameObject> (PuffGroundParticleSystemPrefab).GetComponent<ParticleSystem> ();
			PuffGroundParticleSystem.transform.SetParent (transform);
			SetParticleSystem (PuffGroundParticleSystem, raycastHit, EnginePower);
		}
	}

	void PuffGroundFX_Step ()
	{
		if (PuffGroundParticleSystem != null) {
			RaycastHit raycastHit;
			if (CalcPuffGroundHit (out raycastHit)) {
				SetParticleSystem (PuffGroundParticleSystem, raycastHit, EnginePower);
			}
		}
	}

	void PuffGroundFX_End ()
	{
		if (PuffGroundParticleSystem != null) {
			PuffGroundParticleSystem.loop = false;
			PuffGroundParticleSystem = null;
		}
	}

	bool CalcPuffGroundHit (out RaycastHit raycastHit)
	{
		return Physics.Raycast (transform.position, transform.up, out raycastHit);
	}

	static void SetParticleSystem (ParticleSystem particleSystem, RaycastHit raycastHit, float EnginePower)
	{
		particleSystem.transform.position = raycastHit.point;
		particleSystem.transform.forward = raycastHit.normal;

		float startSizeScale = (1f - raycastHit.distance / 1.5f) * (EnginePower / 30f);

		particleSystem.startSize = startSizeScale;
	}
}
