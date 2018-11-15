using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;

public class InputStatus
{
    public bool isLongPressing;
    public bool isTransform;
    public bool isDraging;
    public bool isPinching;
    public bool isTwisting;
    public Vector2 point;
    public Vector2 prePoint;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float totalPinch;
    public float totalTwist;
    public float pressTime;
}

[Serializable]
public class GestureAction : UnityEvent<InputStatus> { }

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    public GestureAction pressAction;
    public GestureAction pressingAction;
    public GestureAction tapAction;
    public GestureAction longTapAction;
    public GestureAction flickAction;
    public GestureAction dragAction;
    public GestureAction dragingAction;
    public GestureAction pinchAction;
    public GestureAction pinchingAction;
    public GestureAction twistAction;
    public GestureAction twistingAction;

    [SerializeField]
    private bool isDebugLog;
    [SerializeField]
    private float dragBorder = 10.0f;
    [SerializeField]
    private float pinchBorder = 0.2f;
    [SerializeField]
    private float twistBorder = 0.5f;
    [SerializeField]
    private float flickTimeBorder = 0.1f;

    protected bool isLongPressing = false;
    protected bool isTransform = false;
    protected bool isDraging = false;
    protected bool isPinching = false;
    protected bool isTwisting = false;

    protected Vector2 point = Vector2.zero;
    protected Vector2 prePoint = Vector2.zero;
    protected Vector2 startPoint = Vector2.zero;
    protected Vector2 endPoint = Vector2.zero;
    protected float totalPinch = 0;
    protected float totalTwist = 0;
    protected float pressTime = 0;

    protected InputStatus inputStatus;

    protected override void Awake()
    {
        base.isDontDestroyOnLoad = false;
        base.Awake();

        inputStatus = new InputStatus();
    }
    void OnEnable()
    {
        GetComponent<PressGesture>().Pressed += PressHandle;
        GetComponent<LongPressGesture>().LongPressed += LongPressHandle;
        GetComponent<ReleaseGesture>().Released += ReleaseHandle;
        GetComponent<FlickGesture>().Flicked += FlickHandle;
        GetComponent<ScreenTransformGesture>().TransformStarted += TransformStartedHandle;
        GetComponent<ScreenTransformGesture>().StateChanged += StateChangedHandle;
        GetComponent<ScreenTransformGesture>().TransformCompleted += TransformCompletedHandle;
        GetComponent<ScreenTransformGesture>().Cancelled += CancelledHandle;
    }

    private void Update()
    {
        if (IsPressing())
        {
            pressTime += Time.deltaTime;
            ActionInvoke(pressingAction);
        }
    }

    //プレス(押した時)
    protected virtual void PressHandle(object sender, System.EventArgs e)
    {
        Log("PressHandle");
        PressGesture gesture = sender as PressGesture;
        if (IsPressing())
        {
            prePoint = point;
        }
        point = gesture.ScreenPosition;
        startPoint = point;
        ActionInvoke(pressAction);
    }

    //プレス(長押し)
    protected virtual void LongPressHandle(object sender, System.EventArgs e)
    {
        Log("LongPressHandle");
        isLongPressing = true;
    }

    //リリース(離した時)
    protected virtual void ReleaseHandle(object sender, System.EventArgs e)
    {
        if (isTransform) return;

        Log("ReleaseHandle");
        if (isLongPressing)
        {
            ActionInvoke(longTapAction);
        } else
        {
            ActionInvoke(tapAction);
        }

        ResetState();
    }


    //フリック
    protected virtual void FlickHandle(object sender, System.EventArgs e)
    {
        Log("FlickHandle");
        ActionInvoke(flickAction);
    }

    protected virtual void TransformStartedHandle(object sender, System.EventArgs e)
    {
        // 変形開始のタッチ時の処理
        Log("TransformStartedHandle");
        isTransform = true;
    }

    protected virtual void StateChangedHandle(object sender, System.EventArgs e)
    {
        // 変形中のタッチ時の処理
        ScreenTransformGesture gesture = sender as ScreenTransformGesture;
        Vector2 nowPoint = gesture.ScreenPosition;
        if (gesture.NumPointers == 1)
        {
            //Debug.Log(point + " >> " + gesture.ScreenPosition + "## " + gesture.DeltaPosition);
            if (dragBorder <= Vector2.Distance(point, gesture.ScreenPosition) || isDraging)
            {
                //ドラッグ
                isDraging = true;
                ActionInvoke(dragingAction);
            }
        }
        else if (gesture.NumPointers == 2)
        {
            totalPinch += gesture.DeltaScale - 1;
            totalTwist += gesture.DeltaRotation;
            if ((pinchBorder <= Mathf.Abs(totalPinch) || isPinching) && !isTwisting)
            {
                //ピンチイン・アウト
                isPinching = true;
                ActionInvoke(pinchingAction);
            }
            else if ((twistBorder <= Mathf.Abs(totalTwist) || isTwisting) && !isPinching)
            {
                //回転
                isTwisting = true;
                ActionInvoke(twistingAction);
            }
        }
    }

    protected virtual void TransformCompletedHandle(object sender, System.EventArgs e)
    {
        // 変形終了のタッチ時の処理
        if (isDraging)
        {
            if (pressTime > flickTimeBorder)
            {
                Log("drag");
                ActionInvoke(dragAction);
            }
            else
            {
                Log("flick");
                ActionInvoke(flickAction);
            }
        } else if (isPinching)
        {
            Log("pinch");
            ActionInvoke(pinchAction);
        }
        else if (isTwisting)
        {
            Log("twist");
            ActionInvoke(twistAction);
        }
        ResetState();
    }
    protected virtual void CancelledHandle(object sender, System.EventArgs e)
    {
        // 変形終了のタッチ時の処理
        Log("CancelledHandle");
        ResetState();
    }

    protected void ResetState()
    {
        isLongPressing = false;
        isTransform = false;
        isDraging = false;
        isPinching = false;
        isTwisting = false;
        point = Vector2.zero;
        prePoint = Vector2.zero;
        startPoint = Vector2.zero;
        endPoint = Vector2.zero;
        totalPinch = 0;
        totalTwist = 0;
        pressTime = 0;
    }

    protected void SetInputStatus()
    {
        inputStatus.isLongPressing = isLongPressing;
        inputStatus.isTransform = isTransform;
        inputStatus.isDraging = isDraging;
        inputStatus.isPinching = isPinching;
        inputStatus.isTwisting = isTwisting;
        inputStatus.point = ChangeWorldVector(point);
        inputStatus.prePoint = ChangeWorldVector(prePoint);
        inputStatus.startPoint = ChangeWorldVector(startPoint);
        inputStatus.endPoint = ChangeWorldVector(endPoint);
        inputStatus.totalPinch = totalPinch;
        inputStatus.totalTwist = totalTwist;
        inputStatus.pressTime = pressTime;
    }

    protected void ActionInvoke(GestureAction action)
    {
        if (action == null || action.GetPersistentEventCount() == 0) return;
        SetInputStatus();
        action.Invoke(inputStatus);
    }

    protected Vector2 ChangeWorldVector(Vector2 v)
    {
        return Camera.main.ScreenToWorldPoint(v);
    }

    protected bool IsPressing()
    {
        return point != Vector2.zero;
    }

    protected void Log(object obj)
    {
        if (!isDebugLog) return; 
        Debug.Log(obj);
    }
}