using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
	public PlayerController Player;

	#region EasyTouch Subscribe

	public void SubscribeEasyTouchEvents ()
	{
		EasyTouch.On_SwipeStart += EasyTouch_On_SwipeStart;
		EasyTouch.On_Swipe += EasyTouch_On_Swipe;
		EasyTouch.On_SwipeEnd += EasyTouch_On_SwipeEnd;
	}

	public void UnsubscribeEasyTouchEvents ()
	{
		EasyTouch.On_SwipeStart -= EasyTouch_On_SwipeStart;
		EasyTouch.On_Swipe -= EasyTouch_On_Swipe;
		EasyTouch.On_SwipeEnd -= EasyTouch_On_SwipeEnd;
	}

	#endregion

	#region EasyTouch Control

	void EasyTouch_On_SwipeStart (Gesture gesture)
	{
	}

	void EasyTouch_On_Swipe (Gesture gesture)
	{
		var inputVec = gesture.position - gesture.startPosition;
		Player.InputVector (inputVec);
	}

	void EasyTouch_On_SwipeEnd (Gesture gesture)
	{
		Player.InputVector (Vector2.zero);
	}

	#endregion

	void Start ()
	{
		SubscribeEasyTouchEvents ();
	}

	void OnDisable ()
	{
		UnsubscribeEasyTouchEvents ();
	}

	void OnDestroy ()
	{
		UnsubscribeEasyTouchEvents ();
	}
}
