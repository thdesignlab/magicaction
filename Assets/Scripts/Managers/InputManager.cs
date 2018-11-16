using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using TouchScript.Gestures;
using TouchScript.Gestures.TransformGestures;
using TouchScript.Pointers;

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
    [SerializeField]
    private GameObject pointer;
    [SerializeField]
    private GameObject pointerLong;
    [SerializeField]
    private GameObject pointerDouble;
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

    //ジェスチャーアクション
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

    //一時格納
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
    protected Camera mainCam;
    protected GameObject pointerObj;
    protected GameObject pointerLongObj;
    protected LineRenderer pointerLine;
    protected GameObject pointerDoubleObj;
    protected Transform pointerDoubleStartTran;
    protected Transform pointerDoubleEndTran;
    protected LineRenderer pointerDoubleLine;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        inputStatus = new InputStatus();
        mainCam = Camera.main;
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
            prePoint = point;
            point = Input.mousePosition;
            if (!isPinching && !isTwisting)
            {
                SetPointer(point);
                ActionInvoke(pressingAction);
            }
        }
    }

    //ポインター切替
    protected void SetPointer(Vector2 pos = default(Vector2))
    {
        if (Common.FUNC.IsNanVector(pos)) return;

        if (isLongPressing)
        {
            endPoint = pos;
        } else
        {
            startPoint = pos;
        }

        if (pointer == null) return;

        if (pos == default(Vector2))
        {
            //off
            if (pointerObj != null) pointerObj.SetActive(false);
        }
        else
        {
            //on
            if (pointerObj == null)
            {
                pointerObj = Instantiate(pointer);
            }
            pointerObj.SetActive(true);
            pointerObj.transform.position = ChangeWorldVector(pos);
            if (pointerLine != null && isLongPressing)
            {
                pointerLine.SetPosition(1, ChangeWorldVector(endPoint) - ChangeWorldVector(startPoint));
            }
        }
    }
    protected void SetPointerLong(Vector2 pos = default(Vector2))
    {
        if (Common.FUNC.IsNanVector(pos)) return;

        startPoint = pos;

        if (pointerLong == null) return;

        if (pos == default(Vector2))
        {
            //off
            if (pointerLongObj != null) pointerLongObj.SetActive(false);
        }
        else
        {
            //on
            if (pointerLongObj == null)
            {
                pointerLongObj = Instantiate(pointerLong);
                pointerLine = pointerLongObj.GetComponentInChildren<LineRenderer>();
            }
            pointerLongObj.SetActive(true);
            pointerLongObj.transform.position = ChangeWorldVector(pos);
            if (pointerLine != null)
            {
                pointerLine.SetPosition(0, Vector3.zero);
                pointerLine.SetPosition(1, Vector3.zero);
            }
        }
    }
    protected void SetPointerDouble(Vector2 pos1 = default(Vector2), Vector2 pos2 = default(Vector2))
    {
        Debug.Log("SetPointerDouble");
        if (Common.FUNC.IsNanVector(pos1) || Common.FUNC.IsNanVector(pos2)) return;

        SetPointer();
        startPoint = pos1;
        endPoint = pos2;

        if (pointerDouble == null) return;

        if (pos1 == default(Vector2) || pos2 == default(Vector2))
        {
            //off
            if (pointerDoubleObj != null) pointerDoubleObj.SetActive(false);
        }
        else
        {
            //on
            if (pointerDoubleObj == null)
            {
                pointerDoubleObj = Instantiate(pointerDouble);
                pointerDoubleStartTran = pointerDoubleObj.transform.Find("Start");
                pointerDoubleEndTran = pointerDoubleObj.transform.Find("End");
                pointerDoubleLine = pointerDoubleObj.GetComponentInChildren<LineRenderer>();
                pointerDoubleLine.SetPosition(0, Vector2.zero);
            }
            pointerDoubleObj.SetActive(true);
            Vector2 sPos = ChangeWorldVector(startPoint);
            Vector2 ePos = ChangeWorldVector(endPoint);
            pointerDoubleStartTran.position = sPos;
            pointerDoubleEndTran.position = ePos;
            if (pointerDoubleLine != null)
            {
                pointerDoubleLine.SetPosition(0, sPos);
                pointerDoubleLine.SetPosition(1, ePos);
            }
        }
    }
    protected void ResetPointer()
    {
        SetPointer();
        SetPointerLong();
        SetPointerDouble();
    }

    //プレス(押した時)
    protected virtual void PressHandle(object sender, System.EventArgs e)
    {
        Log("PressHandle");
        PressGesture gesture = sender as PressGesture;
        point = gesture.ScreenPosition;
        SetPointer(point);
        ActionInvoke(pressAction);
    }

    //プレス(長押し)
    protected virtual void LongPressHandle(object sender, System.EventArgs e)
    {
        Log("LongPressHandle");
        if (isTransform) return;
        isLongPressing = true;
        SetPointerLong(point);
    }

    //リリース(離した時)
    protected virtual void ReleaseHandle(object sender, System.EventArgs e)
    {
        Log("ReleaseHandle (isTransform="+ isTransform+")");
        if (isTransform) return;

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
        Log("TransformStartedHandle (IsPressing()=" + IsPressing()+")");
        if (!IsPressing()) return;
        isTransform = true;
    }

    protected virtual void StateChangedHandle(object sender, System.EventArgs e)
    {
        Log("StateChangedHandle (isTransform ="+ isTransform+" / IsPressing()=" + IsPressing() + ")");
        if (!isTransform || !IsPressing()) return;

        // 変形中のタッチ時の処理
        ScreenTransformGesture gesture = sender as ScreenTransformGesture;
        Vector2 nowPoint = gesture.ScreenPosition;
        if (Common.FUNC.IsNanVector(nowPoint)) return;

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
            prePoint = point;
            point = nowPoint;

            IList<Pointer> pList = gesture.ActivePointers;
            SetPointerDouble(pList[0].Position, pList[1].Position);
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
        ResetPointer();
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
        return mainCam.ScreenToWorldPoint(v);
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