//using System;
//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Events;
//using TouchScript.Gestures;
//using TouchScript.Gestures.TransformGestures;
//using TouchScript.Pointers;

//public class BattleInputStatus : InputStatus
//{
//    public int pressLevel;
//}

//public class BattleGestureAction : UnityEvent<BattleInputStatus> { }

//public class BattleInputManager : InputManager
//{
//    [SerializeField]
//    private GameObject pointer;
//    [SerializeField]
//    private GameObject pointerStart;
//    [SerializeField]
//    private GameObject pointerLong;
//    [SerializeField]
//    private GameObject pointerDouble;
//    [SerializeField]
//    private bool isDebugLog;
//    [SerializeField]
//    private float dragBorder = 10.0f;
//    [SerializeField]
//    private float pinchBorder = 0.2f;
//    [SerializeField]
//    private float twistBorder = 0.5f;
//    [SerializeField]
//    private float flickTimeBorder = 0.1f;

//    //ジェスチャーアクション
//    protected GestureAction pressAction;
//    protected GestureAction pressingAction;
//    protected GestureAction tapAction;
//    protected GestureAction longTapAction;
//    protected GestureAction flickAction;
//    protected GestureAction dragAction;
//    protected GestureAction dragingAction;
//    protected GestureAction pinchAction;
//    protected GestureAction pinchingAction;
//    protected GestureAction twistAction;
//    protected GestureAction twistingAction;

//    //一時格納
//    protected bool isLongPressing = false;
//    protected bool isTransform = false;
//    protected bool isDraging = false;
//    protected bool isPinching = false;
//    protected bool isTwisting = false;
//    protected Vector2 point = Vector2.zero;
//    protected Vector2 prePoint = Vector2.zero;
//    protected Vector2 startPoint = Vector2.zero;
//    protected Vector2 endPoint = Vector2.zero;
//    protected float totalPinch = 0;
//    protected float totalTwist = 0;
//    protected float pressTime = 0;
//    protected int longPressLevel = 0;

//    protected InputStatus inputStatus;
//    protected Camera mainCam;
//    protected GameObject pointerObj;
//    protected GameObject pointerNormalObj;
//    protected GameObject pointerSpecialObj;
//    protected GameObject pointerStartObj;
//    protected LineRenderer pointerStartLine;
//    protected GameObject pointerLongObj;
//    protected List<GameObject> pointerLongLevelList = new List<GameObject>();
//    protected LineRenderer pointerLongLine;
//    protected GameObject pointerDoubleObj;
//    protected Transform pointerDoubleStartTran;
//    protected Transform pointerDoubleEndTran;
//    protected LineRenderer pointerDoubleLine;
//    protected float longPressThreshold;

//    protected override void Awake()
//    {
//        isDontDestroyOnLoad = false;
//        base.Awake();

//        inputStatus = new InputStatus();
//        mainCam = Camera.main;
//        longPressThreshold = GetComponent<LongPressGesture>().TimeToPress;
//    }
//    void OnEnable()
//    {
//        GetComponent<PressGesture>().Pressed += PressHandle;
//        GetComponent<LongPressGesture>().LongPressed += LongPressHandle;
//        GetComponent<ReleaseGesture>().Released += ReleaseHandle;
//        GetComponent<FlickGesture>().Flicked += FlickHandle;
//        GetComponent<ScreenTransformGesture>().TransformStarted += TransformStartedHandle;
//        GetComponent<ScreenTransformGesture>().StateChanged += StateChangedHandle;
//        GetComponent<ScreenTransformGesture>().TransformCompleted += TransformCompletedHandle;
//        GetComponent<ScreenTransformGesture>().Cancelled += CancelledHandle;
//    }

//    private void Update()
//    {
//        if (IsPressing())
//        {
//            //タップ中処理
//            pressTime += Time.deltaTime;
//            prePoint = point;
//            point = Input.mousePosition;
//            if (!isPinching && !isTwisting)
//            {
//                //ポインター位置
//                SetPointer(point);
//                ActionInvoke(pressingAction);
//            }
//            if (isLongPressing && dragBorder > Vector2.Distance(startPoint, endPoint))
//            {
//                //長押しレベル
//                SetLongPressLevel(Mathf.FloorToInt(pressTime / longPressThreshold));
//            }
//        }
//    }

//    //ポインター切替
//    protected void SetPointer(Vector2 pos = default(Vector2))
//    {
//        if (Common.FUNC.IsNanVector(pos)) return;

//        if (isLongPressing || isTransform)
//        {
//            endPoint = pos;
//        } else
//        {
//            startPoint = pos;
//        }

//        if (pointer == null) return;

//        if (pos == default(Vector2))
//        {
//            //off
//            if (pointerObj != null) pointerObj.SetActive(false);
//            if (pointerStartObj != null) pointerStartObj.SetActive(false);
//            TogglePointerChange(true);
//        }
//        else
//        {
//            //on
//            if (pointerObj == null)
//            {
//                pointerObj = Instantiate(pointer);
//                pointerNormalObj = pointerObj.transform.Find("Normal").gameObject;
//                pointerSpecialObj = pointerObj.transform.Find("Special").gameObject;
//                pointerNormalObj.SetActive(true);
//                pointerSpecialObj.SetActive(false);
//            }
//            pointerObj.SetActive(true);
//            pointerObj.transform.position = ChangeWorldVector(pos);

//            Vector2 sPos = ChangeWorldVector(startPoint);
//            Vector2 ePos = ChangeWorldVector(endPoint);
//            if (isLongPressing)
//            {
//                //ロングタップポインターとのライン
//                if (pointerLongLine != null)
//                {
//                    pointerLongLine.SetPosition(1, ePos - sPos);
//                }
//                TogglePointerChange(false);
//            } else if (isTransform)
//            {
//                //通常タップ時開始位置にポインター生成
//                if (pointerStart != null)
//                {
//                    if (pointerStartObj == null)
//                    {
//                        pointerStartObj = Instantiate(pointerStart);
//                        pointerStartLine = pointerStartObj.GetComponentInChildren<LineRenderer>();
//                    }
//                    pointerStartObj.SetActive(true);
//                    pointerStartObj.transform.position = sPos;
//                    if (pointerStartLine != null)
//                    {
//                        pointerStartLine.SetPosition(0, Vector3.zero);
//                        pointerStartLine.SetPosition(1, ePos - sPos);
//                    }
//                }
//                TogglePointerChange(true);
//            }
//        }
//    }
//    protected void TogglePointerChange(bool isNormal)
//    {
//        if (pointerNormalObj == null && pointerSpecialObj == null) return;
//        pointerNormalObj.SetActive(isNormal);
//        pointerSpecialObj.SetActive(!isNormal);
//    }
//    protected void SetPointerLong(Vector2 pos = default(Vector2))
//    {
//        if (Common.FUNC.IsNanVector(pos)) return;

//        startPoint = pos;

//        if (pointerLong == null) return;

//        if (pos == default(Vector2))
//        {
//            //off
//            if (pointerLongObj != null) pointerLongObj.SetActive(false);
//            TogglePointerChange(true);
//        }
//        else
//        {
//            //on
//            if (pointerLongObj == null)
//            {
//                pointerLongObj = Instantiate(pointerLong);
//                pointerLongObj.FindGameObjectsWithTag("");
//                pointerLongLevelList.Add(pointerLongObj.transform.Find("First").gameObject);
//                pointerLongLevelList.Add(pointerLongObj.transform.Find("Second").gameObject);
//                pointerLongLevelList.Add(pointerLongObj.transform.Find("Third").gameObject);
//                pointerLongLine = pointerLongObj.GetComponentInChildren<LineRenderer>();
//            }
//            pointerLongObj.SetActive(true);
//            SetLongPressLevel(1);
//            pointerLongObj.transform.position = ChangeWorldVector(pos);
//            if (pointerLongLine != null)
//            {
//                pointerLongLine.SetPosition(0, Vector3.zero);
//                pointerLongLine.SetPosition(1, Vector3.zero);
//            }
//            if (pointerStartObj != null) pointerStartObj.SetActive(false);
//        }
//    }

//    protected void SetLongPressLevel(int level)
//    {
//        if (longPressLevel == level || level <= 0) return;
//        if (pointerLongLevelList.Count < level) level = pointerLongLevelList.Count;
//        longPressLevel = level == 0 ? 1 : level;

//        for (int i = 0; i < pointerLongLevelList.Count; i++)
//        {
//            bool flg = (i + 1 == level);
//            pointerLongLevelList[i].SetActive(flg);

//        }
//    }
//    protected void SetPointerDouble(Vector2 pos1 = default(Vector2), Vector2 pos2 = default(Vector2))
//    {
//        if (Common.FUNC.IsNanVector(pos1) || Common.FUNC.IsNanVector(pos2)) return;

//        SetPointer();
//        startPoint = pos1;
//        endPoint = pos2;

//        if (pointerDouble == null) return;

//        if (pos1 == default(Vector2) || pos2 == default(Vector2))
//        {
//            //off
//            if (pointerDoubleObj != null) pointerDoubleObj.SetActive(false);
//            TogglePointerChange(true);
//        }
//        else
//        {
//            //on
//            if (pointerDoubleObj == null)
//            {
//                pointerDoubleObj = Instantiate(pointerDouble);
//                pointerDoubleStartTran = pointerDoubleObj.transform.Find("Start");
//                pointerDoubleEndTran = pointerDoubleObj.transform.Find("End");
//                pointerDoubleLine = pointerDoubleObj.GetComponentInChildren<LineRenderer>();
//                pointerDoubleLine.SetPosition(0, Vector2.zero);
//            }
//            pointerDoubleObj.SetActive(true);
//            Vector2 sPos = ChangeWorldVector(startPoint);
//            Vector2 ePos = ChangeWorldVector(endPoint);
//            pointerDoubleStartTran.position = sPos;
//            pointerDoubleEndTran.position = ePos;
//            if (pointerDoubleLine != null)
//            {
//                pointerDoubleLine.SetPosition(0, sPos);
//                pointerDoubleLine.SetPosition(1, ePos);
//            }
//        }
//    }
//    protected void ResetPointer()
//    {
//        SetPointer();
//        SetPointerLong();
//        SetPointerDouble();
//    }

//    //プレス(押した時)
//    protected virtual void PressHandle(object sender, System.EventArgs e)
//    {
//        Log("PressHandle");
//        PressGesture gesture = sender as PressGesture;
//        point = gesture.ScreenPosition;
//        SetPointer(point);
//        ActionInvoke(pressAction);
//    }

//    //プレス(長押し)
//    protected virtual void LongPressHandle(object sender, System.EventArgs e)
//    {
//        Log("LongPressHandle");
//        if (isTransform) return;
//        isLongPressing = true;
//        SetPointerLong(point);
//    }

//    //リリース(離した時)
//    protected virtual void ReleaseHandle(object sender, System.EventArgs e)
//    {
//        Log("ReleaseHandle (isTransform="+ isTransform+")");
//        if (isTransform) return;

//        if (isLongPressing)
//        {
//            ActionInvoke(longTapAction);
//        } else
//        {
//            ActionInvoke(tapAction);
//        }
//        ResetState();
//    }


//    //フリック
//    protected virtual void FlickHandle(object sender, System.EventArgs e)
//    {
//        Log("FlickHandle");
//        ActionInvoke(flickAction);
//    }

//    protected virtual void TransformStartedHandle(object sender, System.EventArgs e)
//    {
//        // 変形開始のタッチ時の処理
//        Log("TransformStartedHandle (IsPressing()=" + IsPressing()+")");
//        if (!IsPressing()) return;
//        isTransform = true;
//    }

//    protected virtual void StateChangedHandle(object sender, System.EventArgs e)
//    {
//        Log("StateChangedHandle (isTransform ="+ isTransform+" / IsPressing()=" + IsPressing() + ")");
//        if (!isTransform || !IsPressing()) return;

//        // 変形中のタッチ時の処理
//        ScreenTransformGesture gesture = sender as ScreenTransformGesture;
//        Vector2 nowPoint = gesture.ScreenPosition;
//        if (Common.FUNC.IsNanVector(nowPoint)) return;

//        if (gesture.NumPointers == 1)
//        {
//            if (dragBorder <= Vector2.Distance(point, gesture.ScreenPosition) || isDraging)
//            {
//                //ドラッグ
//                isDraging = true;
//                ActionInvoke(dragingAction);
//            }
//        }
//        else if (gesture.NumPointers == 2)
//        {
//            totalPinch += gesture.DeltaScale - 1;
//            totalTwist += gesture.DeltaRotation;
//            if ((pinchBorder <= Mathf.Abs(totalPinch) || isPinching) && !isTwisting)
//            {
//                //ピンチイン・アウト
//                isPinching = true;
//                ActionInvoke(pinchingAction);
//            }
//            else if ((twistBorder <= Mathf.Abs(totalTwist) || isTwisting) && !isPinching)
//            {
//                //回転
//                isTwisting = true;
//                ActionInvoke(twistingAction);
//            }
//            prePoint = point;
//            point = nowPoint;

//            IList<Pointer> pList = gesture.ActivePointers;
//            SetPointerDouble(pList[0].Position, pList[1].Position);
//        }
//    }

//    protected virtual void TransformCompletedHandle(object sender, System.EventArgs e)
//    {
//        // 変形終了のタッチ時の処理
//        if (isDraging)
//        {
//            if (pressTime > flickTimeBorder)
//            {
//                Log("drag");
//                ActionInvoke(dragAction);
//            }
//            else
//            {
//                Log("flick");
//                ActionInvoke(flickAction);
//            }
//        } else if (isPinching)
//        {
//            Log("pinch");
//            ActionInvoke(pinchAction);
//        }
//        else if (isTwisting)
//        {
//            Log("twist");
//            ActionInvoke(twistAction);
//        }
//        ResetState();
//    }
//    protected virtual void CancelledHandle(object sender, System.EventArgs e)
//    {
//        // 変形終了のタッチ時の処理
//        Log("CancelledHandle");
//        ResetState();
//    }

//    protected void ResetState()
//    {
//        ResetPointer();
//        isLongPressing = false;
//        isTransform = false;
//        isDraging = false;
//        isPinching = false;
//        isTwisting = false;
//        point = Vector2.zero;
//        prePoint = Vector2.zero;
//        startPoint = Vector2.zero;
//        endPoint = Vector2.zero;
//        totalPinch = 0;
//        totalTwist = 0;
//        pressTime = 0;
//        longPressLevel = 0;
//    }

//    protected void SetInputStatus()
//    {
//        inputStatus.isLongPressing = isLongPressing;
//        inputStatus.isTransform = isTransform;
//        inputStatus.isDraging = isDraging;
//        inputStatus.isPinching = isPinching;
//        inputStatus.isTwisting = isTwisting;
//        inputStatus.point = point;
//        inputStatus.prePoint = prePoint;
//        inputStatus.startPoint = startPoint;
//        inputStatus.endPoint = endPoint;
//        inputStatus.totalPinch = totalPinch;
//        inputStatus.totalTwist = totalTwist;
//        inputStatus.pressTime = pressTime;
//        inputStatus.longPressLevel = longPressLevel;
//    }

//    protected void ActionInvoke(GestureAction action)
//    {
//        if (action == null) return;

//        SetInputStatus();
//        action.Invoke(inputStatus);
//    }

//    public Vector2 ChangeWorldVector(Vector2 v)
//    {
//        return mainCam.ScreenToWorldPoint(v);
//    }

//    protected bool IsPressing()
//    {
//        return point != Vector2.zero;
//    }

//    protected void Log(object obj)
//    {
//        if (!isDebugLog) return; 
//        Debug.Log(obj);
//    }

//    private GestureAction CreateGestureAction(UnityAction<InputStatus> action)
//    {
//        GestureAction gesture = new GestureAction();
//        gesture.AddListener(action);
//        return gesture;
//    }
//    public void SetPressAction(UnityAction<InputStatus> action)
//    {
//        pressAction = CreateGestureAction(action);
//    }
//    public void SetPressingAction(UnityAction<InputStatus> action)
//    {
//        pressingAction = CreateGestureAction(action);
//    }
//    public void SetTapAction(UnityAction<InputStatus> action)
//    {
//        tapAction = CreateGestureAction(action);
//    }
//    public void SetLongTapAction(UnityAction<InputStatus> action)
//    {
//        longTapAction = CreateGestureAction(action);
//    }
//    public void SetFlickAction(UnityAction<InputStatus> action)
//    {
//        flickAction = CreateGestureAction(action);
//    }
//    public void SetDragAction(UnityAction<InputStatus> action)
//    {
//        dragAction = CreateGestureAction(action);
//    }
//    public void SetDragingAction(UnityAction<InputStatus> action)
//    {
//        dragingAction = CreateGestureAction(action);
//    }
//    public void SetPinchAction(UnityAction<InputStatus> action)
//    {
//        pinchAction = CreateGestureAction(action);
//    }
//    public void SetPinchingAction(UnityAction<InputStatus> action)
//    {
//        pinchingAction = CreateGestureAction(action);
//    }
//    public void SetTwistAction(UnityAction<InputStatus> action)
//    {
//        twistAction = CreateGestureAction(action);
//    }
//    public void SetTwistingAction(UnityAction<InputStatus> action)
//    {
//        twistingAction = CreateGestureAction(action);
//    }
//}