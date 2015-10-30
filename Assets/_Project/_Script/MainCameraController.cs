using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour
{
	public PlayerController player;
	public float LerpRate;
	public Vector3 CameraOriginZeroPoint;

	void LateUpdate ()
	{
		SetCameraPosition ();
	}

	void SetCameraPosition ()
	{
		Vector3 playerCameraSurfPoint = new Vector3 (player.transform.position.x, player.transform.position.y, CameraOriginZeroPoint.z);
		transform.position = Vector3.Lerp (playerCameraSurfPoint, CameraOriginZeroPoint, LerpRate);
	}
}
