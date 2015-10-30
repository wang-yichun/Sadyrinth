using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public PlayerController Player;
	public ControlPadController Pad;
	public Transform CollectableContainerTransform;

	public int MaxSadyCount;
	public int CurSadyCount;

	private static GameController MyGameController;

	public static GameController GetInstance ()
	{
		return MyGameController;
	}

	#region EasyTouch Subscribe

	public void SubscribeEasyTouchEvents ()
	{
		EasyTouch.On_SwipeStart += EasyTouch_On_SwipeStart;
		EasyTouch.On_Swipe += EasyTouch_On_Swipe;
		EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;

		EasyTouch.On_TouchStart += EasyTouch_On_TouchStart;
		EasyTouch.On_TouchUp += EasyTouch_On_TouchUp;
	}

	public void UnsubscribeEasyTouchEvents ()
	{
		EasyTouch.On_SwipeStart -= EasyTouch_On_SwipeStart;
		EasyTouch.On_Swipe -= EasyTouch_On_Swipe;
		EasyTouch.On_SwipeEnd -= EasyTouch_On_SwipeEnd;

		EasyTouch.On_TouchStart -= EasyTouch_On_TouchStart;
		EasyTouch.On_TouchUp -= EasyTouch_On_TouchUp;
	}

	void EasyTouch_On_TouchStart (Gesture gesture)
	{
		Pad.Open ();
		Pad.SetPadPosition (gesture.GetTouchToWorldPoint (0f));
	}

	void EasyTouch_On_TouchUp (Gesture gesture)
	{
		Pad.Close ();
	}

	#endregion

	#region EasyTouch Control


	void EasyTouch_On_SwipeStart (Gesture gesture)
	{
		OnSwipeStartPosition = gesture.startPosition;
		Pad.Open ();
		Pad.SetPadPosition (gesture.GetTouchToWorldPoint (0f), true);
	}

	Vector2 OnSwipeStartPosition;

	void EasyTouch_On_Swipe (Gesture gesture)
	{
		var inputVec = gesture.position - OnSwipeStartPosition;
		Player.InputVector (inputVec);
	}

	void EasyTouch_On_SwipeEnd (Gesture gesture)
	{
		Pad.Close ();
		Player.InputVector (Vector2.zero);
	}

	#endregion

	void InitUIData ()
	{
		// 燃料
		NotificationCenter.DefaultCenter.PostNotification (this, "set_fuel_to",
			new Hashtable () { 
				{ "value", 0f }
			}
		);
		// Sady
		MaxSadyCount = CollectableContainerTransform.childCount;
		CurSadyCount = MaxSadyCount;
	}

	void Awake ()
	{
		MyGameController = this;
	}

	void Start ()
	{
		ResetGame ();
		StartGame ();
	}

	void OnDisable ()
	{
		UnsubscribeEasyTouchEvents ();
	}

	void OnDestroy ()
	{
		UnsubscribeEasyTouchEvents ();
	}

	#region 游戏总控

	void ResetGame ()
	{
		InitUIData ();
		Player.InitPlayer ();
	}

	void StartGame ()
	{
		SubscribeEasyTouchEvents ();
	}

	void EndGame ()
	{
		UnsubscribeEasyTouchEvents ();
	}

	#endregion
}
