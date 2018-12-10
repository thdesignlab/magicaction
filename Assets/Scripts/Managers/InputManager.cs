using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
    public bool isLongDraging;
    public bool isTapPlayer;
    public Vector2 point;
    public Vector2 prePoint;
    public Vector2 startPoint;
    public Vector2 endPoint;
    public float totalPinch;
    public float totalTwist;
    public float pressTime;
    public int pressLevel;
    public GameObject tapEnemy;
    public List<Vector2> linePositions;

    public Vector2 GetPoint()
    {
        return InputManager.Instance.ChangeWorldVector(point);
    }
    public Vector2 GetPrePoint()
    {
        return InputManager.Instance.ChangeWorldVector(prePoint);
    }
    public Vector2 GetStartPoint()
    {
        return InputManager.Instance.ChangeWorldVector(startPoint);
    }
    public Vector2 GetEndPoint()
    {
        return InputManager.Instance.ChangeWorldVector(endPoint);
    }
    public bool IsTapEnemy()
    {
        return InputManager.Instance.IsTapEnemy(point);
    }
    public Transform GetTapEnemyTran()
    {
        if (tapEnemy == null) return null;
        return tapEnemy.transform;
    }
}

public class GestureAction : UnityEvent<InputStatus> { }

public class GesturePointer
{
    public GameObject body;
    public List<GameObject> levelBodyList = new List<GameObject>();
    public List<LineRenderer> levelLineList = new List<LineRenderer>();
    public bool isMultiPoints = false;

    public GameObject GetLevelBody(int level)
    {
        if (levelBodyList.Count < level) return null;
        return levelBodyList[level];
    }
    public void ResetLevelLine()
    {
        foreach (LineRenderer line in levelLineList)
        {
            if (line == null) continue;
            line.positionCount = 1;
            line.SetPosition(0, Vector2.zero);
        }
        InputManager.Instance.linePositionList = new List<Vector2>();
    }
    public void AddLevelLine(Vector2 pos, bool isTapPlayer = false)
    {
        foreach (LineRenderer line in levelLineList)
        {
            if (line == null) continue;
            if (isMultiPoints && !isTapPlayer)
            {
                Vector2 preLine = line.GetPosition(line.positionCount - 1);
                if (InputManager.Instance.GetFixDistance(preLine, pos) < 0.5f) continue;
                line.positionCount += 1;
                line.SetPosition(line.positionCount - 1, pos);
                InputManager.Instance.linePositionList.Add(pos);
            }
            else
            {
                line.positionCount = 2;
                line.SetPosition(1, pos);
                if (InputManager.Instance.linePositionList.Count == 0)
                {
                    InputManager.Instance.linePositionList.Add(pos);
                } else
                {
                    InputManager.Instance.linePositionList[0] =pos;
                }
            }
        }
    }
}

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [SerializeField]
    private GameObject pointer;
    [SerializeField]
    private GameObject pointerStart;
    [SerializeField]
    private GameObject pointerLong;
    [SerializeField]
    private GameObject pointerDouble;
    [SerializeField]
    private bool isMultiLineStartPointer;
    [SerializeField]
    private bool isMultiLineLongPointer;
    [SerializeField]
    private bool isDebugLog;
    [SerializeField]
    private List<float> longPressBorder = new List<float>();
    [SerializeField]
    private float dragBorder = 10.0f;
    [SerializeField]
    private float pinchBorder = 0.2f;
    [SerializeField]
    private float twistBorder = 0.5f;
    [SerializeField]
    private float flickTimeBorder = 0.1f;

    //ジェスチャーアクション
    protected GestureAction pressAction;
    protected GestureAction pressingAction;
    protected GestureAction tapAction;
    protected GestureAction longTapAction;
    protected GestureAction longTapLevelAction;
    protected GestureAction releaseAction;
    protected GestureAction flickAction;
    protected GestureAction dragAction;
    protected GestureAction dragingAction;
    protected GestureAction pinchAction;
    protected GestureAction pinchingAction;
    protected GestureAction twistAction;
    protected GestureAction twistingAction;

    //一時格納
    protected bool isLongPressing
    {
        get { return inputStatus.isLongPressing; }
        set { inputStatus.isLongPressing = value; }
    }
    protected bool isTransform
    {
        get { return inputStatus.isTransform; }
        set { inputStatus.isTransform = value; }
    }
    protected bool isDraging
    {
        get { return inputStatus.isDraging; }
        set { inputStatus.isDraging = value; }
    }
    protected bool isPinching
    {
        get { return inputStatus.isPinching; }
        set { inputStatus.isPinching = value; }
    }
    protected bool isTwisting
    {
        get { return inputStatus.isTwisting; }
        set { inputStatus.isTwisting = value; }
    }
    protected bool isLongDraging
    {
        get { return inputStatus.isLongDraging; }
        set { inputStatus.isLongDraging = value; }
    }
    protected bool isTapPlayer
    {
        get { return inputStatus.isTapPlayer; }
        set { inputStatus.isTapPlayer = value; }
    }
    protected Vector2 point
    {
        get { return inputStatus.point; }
        set { inputStatus.point = value; }
    }
    protected Vector2 prePoint
    {
        get { return inputStatus.prePoint; }
        set { inputStatus.prePoint = value; }
    }
    protected Vector2 startPoint
    {
        get { return inputStatus.startPoint; }
        set { inputStatus.startPoint = value; }
    }
    protected Vector2 endPoint
    {
        get { return inputStatus.endPoint; }
        set { inputStatus.endPoint = value; }
    }
    protected float totalPinch
    {
        get { return inputStatus.totalPinch; }
        set { inputStatus.totalPinch = value; }
    }
    protected float totalTwist
    {
        get { return inputStatus.totalTwist; }
        set { inputStatus.totalTwist = value; }
    }
    protected float pressTime
    {
        get { return inputStatus.pressTime; }
        set { inputStatus.pressTime = value; }
    }
    protected int pressLevel
    {
        get { return inputStatus.pressLevel; }
        set { inputStatus.pressLevel = value; }
    }
    protected GameObject tapEnemy
    {
        get { return inputStatus.tapEnemy; }
        set { inputStatus.tapEnemy = value; }
    }

    protected InputStatus inputStatus;
    protected Camera mainCam;
    protected GesturePointer pointerObj;
    protected GesturePointer pointerStartObj;
    protected GesturePointer pointerLongObj;
    protected GameObject pointerDoubleObj;
    protected Transform pointerDoubleStartTran;
    protected Transform pointerDoubleEndTran;
    protected LineRenderer pointerDoubleLine;
    protected bool isActive = false;
    protected bool isTapUI = false;
    public List<Vector2> linePositionList = new List<Vector2>();

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        inputStatus = new InputStatus();
        mainCam = Camera.main;

        if (longPressBorder.Count > 0)
        {
            //長押し時間設定
            GetComponent<LongPressGesture>().TimeToPress = longPressBorder[0];
        }
        //pointer格納
        pointerObj = GetPointer(pointer);
        pointerStartObj = GetPointer(pointerStart, isMultiLineStartPointer);
        pointerLongObj = GetPointer(pointerLong, isMultiLineLongPointer);
        ResetState(false);
    }

    public void SetActive(bool flg)
    {
        isActive = flg;
        if (flg)
        {
            AddEvent();
        } else
        {
            RemoveEvent();
        }
    }

    protected GesturePointer GetPointer(GameObject p, bool isMultiPoints = false)
    {
        if (p == null) return null;
        GesturePointer newPointer = new GesturePointer();
        newPointer.isMultiPoints = isMultiPoints;
        newPointer.body = Instantiate(p);
        for (int i = 0; i <= longPressBorder.Count; i++)
        {
            Transform bodyTran = newPointer.body.transform.Find(Common.CO.LEVEL_PREFIX + i.ToString());
            if (bodyTran == null)
            {
                newPointer.levelBodyList.Add(null);
                newPointer.levelLineList.Add(null);
                continue;
            }
            newPointer.levelBodyList.Add(bodyTran.gameObject);
            LineRenderer line = bodyTran.GetComponentInChildren<LineRenderer>();
            newPointer.levelLineList.Add(line);
        }
        newPointer.ResetLevelLine();
        newPointer.body.SetActive(false);
        return newPointer;
    }

    protected void AddEvent()
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
    protected void RemoveEvent()
    {
        GetComponent<PressGesture>().Pressed -= PressHandle;
        GetComponent<LongPressGesture>().LongPressed -= LongPressHandle;
        GetComponent<ReleaseGesture>().Released -= ReleaseHandle;
        GetComponent<FlickGesture>().Flicked -= FlickHandle;
        GetComponent<ScreenTransformGesture>().TransformStarted -= TransformStartedHandle;
        GetComponent<ScreenTransformGesture>().StateChanged -= StateChangedHandle;
        GetComponent<ScreenTransformGesture>().TransformCompleted -= TransformCompletedHandle;
        GetComponent<ScreenTransformGesture>().Cancelled -= CancelledHandle;
    }

    private void Update()
    {
        if (!IsActive()) return;

        if (IsPressing())
        {
            ActionInvoke(pressingAction);

            //タップ中処理
            pressTime += Time.deltaTime;
            prePoint = point;
            point = Input.mousePosition;
            if (!isPinching && !isTwisting)
            {
                //ポインター位置
                SetPointer(point);
            }
            if (isLongPressing)
            {
                if (isLongDraging)
                {
                    ActionInvoke(dragingAction);
                }
                else
                {
                    if (dragBorder > GetFixDistance(startPoint, endPoint))
                    {
                        //長押しレベル
                        SetPressLevel();
                    }
                    else
                    {
                        isLongDraging = true;
                    }
                }
            }
        }
    }

    //通常時,ドラッグ時ポインター
    protected void SetPointer(Vector2 pos = default(Vector2))
    {
        if (Common.FUNC.IsNanVector(pos)) return;

        if (isLongPressing || isTransform)
        {
            endPoint = pos;
        } else
        {
            startPoint = pos;
        }

        if (pointerObj == null || pointerObj.body == null) return;

        if (pos == default(Vector2))
        {
            //off
            pointerObj.body.SetActive(false);
            if (pointerStartObj != null)
            {
                pointerStartObj.body.SetActive(false);
                pointerStartObj.ResetLevelLine();
            }
            if (pointerLongObj != null)
            {
                pointerLongObj.body.SetActive(false);
                pointerLongObj.ResetLevelLine();
            }
        }
        else
        {
            //on
            pointerObj.body.SetActive(true);
            pointerObj.body.transform.position = ChangeWorldVector(pos);

            Vector2 sPos = ChangeWorldVector(startPoint);
            Vector2 ePos = ChangeWorldVector(endPoint);
            if (isLongPressing)
            {
                //長押し時ライン
                if (pointerLongObj != null)
                {
                    pointerLongObj.AddLevelLine(ePos - sPos, isTapPlayer);
                }
            } else if (isTransform)
            {
                //通常タップ時ポインター・ライン
                if (pointerStartObj != null)
                {
                    pointerStartObj.body.SetActive(true);
                    pointerStartObj.body.transform.position = sPos;
                    pointerStartObj.AddLevelLine(ePos - sPos, isTapPlayer);
                }
            }
        }
    }

    //長押し時ポインター
    protected void SetPointerLong(Vector2 pos = default(Vector2))
    {
        if (Common.FUNC.IsNanVector(pos)) return;

        startPoint = pos;

        if (pointerLongObj == null || pointerLongObj.body == null) return;

        if (pos == default(Vector2))
        {
            //off
            pointerLongObj.body.SetActive(false);
        }
        else
        {
            //on
            pointerLongObj.body.SetActive(true);
            pointerLongObj.body.transform.position = ChangeWorldVector(pos);
            pointerLongObj.ResetLevelLine();
            if (pointerStartObj != null) pointerStartObj.body.SetActive(false);
        }
    }

    //長押しレベル更新
    protected void SetPressLevel()
    {
        int level = 0;
        for (int i = longPressBorder.Count; i >= 0; i--)
        {
            if (i == 0 || pressTime >= longPressBorder[i - 1])
            {
                level = i;
                break;
            }
        }
        SetPressLevel(level);
    }
    protected void SetPressLevel(int level)
    {
        if (pressLevel == level) return;
        pressLevel = level;
        SetPointerLevel(pointerObj);
        SetPointerLevel(pointerStartObj);
        SetPointerLevel(pointerLongObj);
        ActionInvoke(longTapLevelAction);
    }

    //長押しレベルによるポインター表示変更
    protected void SetPointerLevel(GesturePointer p)
    {
        if (p == null || (!p.body.activeInHierarchy && pressLevel > 0)) return;

        GameObject activeBody = p.levelBodyList[pressLevel];
        GameObject stockBody = null;
        for (int i = 0; i < p.levelBodyList.Count; i++)
        {
            if (p.levelBodyList[i] == null) continue;
            if (stockBody == null) stockBody = p.levelBodyList[i];
            p.levelBodyList[i].SetActive(false);
        }
        if (activeBody == null) activeBody = stockBody;
        if (activeBody != null) activeBody.SetActive(true);
    }

    //pinch,twist時ポインター
    protected void SetPointerDouble(Vector2 pos1 = default(Vector2), Vector2 pos2 = default(Vector2))
    {
        if (Common.FUNC.IsNanVector(pos1) || Common.FUNC.IsNanVector(pos2)) return;

        SetPointer();
        SetPointerLong();
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
        if (!IsActive()) return;

        Log("PressHandle");
        PressGesture gesture = sender as PressGesture;
        point = gesture.ScreenPosition;
        if (IsTapUI(point))
        {
            isTapUI = true;
            return;
        }

        isTapPlayer = IsTapPlayer(point);
        SetPointer(point);
        ActionInvoke(pressAction);
    }

    //プレス(長押し)
    protected virtual void LongPressHandle(object sender, System.EventArgs e)
    {
        if (!IsActive()) return;

        if (isTransform || longPressBorder.Count == 0) return;
        Log("LongPressHandle");
        isLongPressing = true;
        SetPointerLong(point);
    }

    //リリース(離した時)
    protected virtual void ReleaseHandle(object sender, System.EventArgs e)
    {
        if (!IsActive())
        {
            isTapUI = false;
            return;
        }

        Log("ReleaseHandle (isTransform=" + isTransform + ")");
        if (isTransform) return;

        if (isLongDraging)
        {
            Log("LongDraging");
            ActionInvoke(dragAction);
        }
        else if (isLongPressing)
        {
            Log("LongTap");
            ActionInvoke(longTapAction);
        }
        else
        {
            Log("Tap");
            ActionInvoke(tapAction);
        }
        ResetState();
    }

    //フリック
    protected virtual void FlickHandle(object sender, System.EventArgs e)
    {
        if (!IsActive()) return;

        Log("FlickHandle");
        ActionInvoke(flickAction);
    }

    protected virtual void TransformStartedHandle(object sender, System.EventArgs e)
    {
        if (!IsActive()) return;

        // 変形開始のタッチ時の処理
        if (!IsPressing()) return;
        Log("TransformStartedHandle (IsPressing()=" + IsPressing() + ")");
        isTransform = true;
    }

    protected virtual void StateChangedHandle(object sender, System.EventArgs e)
    {
        if (!IsActive()) return;

        if (!isTransform || !IsPressing()) return;
        Log("StateChangedHandle (isTransform =" + isTransform + " / IsPressing()=" + IsPressing() + ")");

        // 変形中のタッチ時の処理
        ScreenTransformGesture gesture = sender as ScreenTransformGesture;
        Vector2 nowPoint = gesture.ScreenPosition;
        if (Common.FUNC.IsNanVector(nowPoint)) return;

        if (gesture.NumPointers == 1)
        {
            if (dragBorder <= GetFixDistance(startPoint, nowPoint) || isDraging)
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
        if (!IsActive()) return;

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
        if (!IsActive()) return;

        // 変形終了のタッチ時の処理
        Log("CancelledHandle");
        ResetState();
    }

    protected void ResetState(bool isAction = true)
    {
        if (isAction) ActionInvoke(releaseAction);
        ResetPointer();
        isLongPressing = false;
        isTransform = false;
        isDraging = false;
        isPinching = false;
        isTwisting = false;
        isLongDraging = false;
        isTapPlayer = false;
        point = Vector2.zero;
        prePoint = Vector2.zero;
        startPoint = Vector2.zero;
        endPoint = Vector2.zero;
        totalPinch = 0;
        totalTwist = 0;
        pressTime = 0;
        SetPressLevel(0);
        linePositionList = new List<Vector2>();
        isTapUI = false;
        tapEnemy = null;
    }

    protected void SetInputStatus()
    {
        inputStatus.linePositions = linePositionList;
    }

    protected void ActionInvoke(GestureAction action)
    {
        if (action == null) return;

        SetInputStatus();
        action.Invoke(inputStatus);
    }

    public Vector2 ChangeWorldVector(Vector2 v)
    {
        return Common.FUNC.ChangeWorldVector(v, mainCam);
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

    //指定解像度換算した２点間距離
    public float GetFixDistance(Vector2 pos1, Vector2 pos2)
    {
        return (pos1 - pos2).magnitude / ScreenManager.Instance.GetSizeRate();
    }

    //指定座標のオブジェクト取得
    public GameObject GetTapObject(Vector2 pos, int layerMask = 0, Camera cam = null)
    {
        GameObject obj = null;
        cam = cam ?? Camera.main;
        Ray ray = cam.ScreenPointToRay(pos);
        RaycastHit2D hit2d = Physics2D.Raycast(ray.origin, ray.direction, 10, layerMask);
        if (hit2d)
        {
            obj = hit2d.transform.gameObject;
        }
        return obj;
    }
    public bool IsTapPlayer(Vector2 pos)
    {
        int layerMask = Common.FUNC.GetLayerMask(Common.CO.LAYER_PLAYER_BODY);
        GameObject obj = GetTapObject(pos, layerMask);
        return (obj != null);
    }
    public bool IsTapEnemy(Vector2 pos)
    {
        string[] tags = new string[] { Common.CO.LAYER_ENEMY, Common.CO.LAYER_ENEMY_BOSS, Common.CO.LAYER_ENEMY_BODY };
        int layerMask = Common.FUNC.GetLayerMask(tags);
        tapEnemy = GetTapObject(pos, layerMask);
        return (tapEnemy != null);
    }

    //指定座標のUI取得
    public List<GameObject> GetTapUIList(Vector2 pos)
    {
        PointerEventData p = new PointerEventData(EventSystem.current);
        p.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(p, results);
        List<GameObject> objList = new List<GameObject>();
        foreach (RaycastResult hit in results)
        {
            objList.Add(hit.gameObject);
        }
        return objList;
    }
    public bool IsTapUI(Vector2 pos)
    {
        List<GameObject> objList = GetTapUIList(pos);
        return (objList.Count > 0);
    }

    private bool IsActive()
    {
        return (isActive && !isTapUI);
    }

    //### GestureAction登録 ###

    private GestureAction CreateGestureAction(UnityAction<InputStatus> action)
    {
        GestureAction gesture = new GestureAction();
        gesture.AddListener(action);
        return gesture;
    }
    public void SetPressAction(UnityAction<InputStatus> action)
    {
        pressAction = CreateGestureAction(action);
    }
    public void SetPressingAction(UnityAction<InputStatus> action)
    {
        pressingAction = CreateGestureAction(action);
    }
    public void SetTapAction(UnityAction<InputStatus> action)
    {
        tapAction = CreateGestureAction(action);
    }
    public void SetLongTapAction(UnityAction<InputStatus> action)
    {
        longTapAction = CreateGestureAction(action);
    }
    public void SetLongTapLevelAction(UnityAction<InputStatus> action)
    {
        longTapLevelAction = CreateGestureAction(action);
    }
    public void SetReleaseAction(UnityAction<InputStatus> action)
    {
        releaseAction = CreateGestureAction(action);
    }
    public void SetFlickAction(UnityAction<InputStatus> action)
    {
        flickAction = CreateGestureAction(action);
    }
    public void SetDragAction(UnityAction<InputStatus> action)
    {
        dragAction = CreateGestureAction(action);
    }
    public void SetDragingAction(UnityAction<InputStatus> action)
    {
        dragingAction = CreateGestureAction(action);
    }
    public void SetPinchAction(UnityAction<InputStatus> action)
    {
        pinchAction = CreateGestureAction(action);
    }
    public void SetPinchingAction(UnityAction<InputStatus> action)
    {
        pinchingAction = CreateGestureAction(action);
    }
    public void SetTwistAction(UnityAction<InputStatus> action)
    {
        twistAction = CreateGestureAction(action);
    }
    public void SetTwistingAction(UnityAction<InputStatus> action)
    {
        twistingAction = CreateGestureAction(action);
    }
}