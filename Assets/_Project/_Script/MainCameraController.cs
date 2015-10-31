using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour
{
	public float LerpRate;
	public Vector3 CameraOriginZeroPoint;

	public PlayerController Player;

	void LateUpdate ()
	{
		SetCameraPosition ();
	}

	void SetCameraPosition ()
	{
		if (Player != null) {
			Vector3 playerCameraSurfPoint = new Vector3 (Player.transform.position.x, Player.transform.position.y, CameraOriginZeroPoint.z);
			transform.position = Vector3.Lerp (playerCameraSurfPoint, CameraOriginZeroPoint, LerpRate);
		}
	}
}
