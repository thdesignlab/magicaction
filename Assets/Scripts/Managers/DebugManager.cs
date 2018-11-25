using UnityEngine;
using System.Collections;

public class DebugManager : SingletonMonoBehaviour<DebugManager>
{
    private Queue logQueue = new Queue();
    private int logCount = 100;
    private string preCondition = "";
    private float preLogTime = 0;

    public void AdminLog(object key, object value)
    {
        AdminLog(key + " >> " + value);
    }
    public void AdminLog(object log)
    {
        if (UserManager.isAdmin || AppManager.Instance.isDebug) Debug.Log(log);
    }

    public void StartLog()
    {
        if (!AppManager.Instance.isDebug && !UserManager.isAdmin) return;
        Application.logMessageReceived += HandleLog;
    }
    public void StopLog()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void OnEnable()
    {
        StartLog();
    }
    void OnDisable()
    {
        StopLog();
    }

    void HandleLog(string condition, string stackTrace, LogType type)
    {
        if (condition == preCondition)
        {
            //同じメッセージは続けて出さない
            if (Time.time - preLogTime < 1.0f) return;
        }
        preCondition = condition;
        preLogTime = Time.time;
        //stackTrace += "\n"+UnityEngine.StackTraceUtility.ExtractStackTrace();
        // 必要な変数を宣言する
        //string dtNow = System.DateTime.Now.ToString("yyyy/MM/dd (ddd) HH:mm:ss");
        string dtNow = System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        string trace = stackTrace.Remove(0, (stackTrace.IndexOf("\n") + 1));
        string log = "### START ### -- "+ type.ToString() + " -- " + dtNow + "\n【condition】" + condition + "\n【stackTrace】" + trace + "\n### END ###\n";
        //string log = "### START ### -- " + dtNow + "\n" + stackTrace + "\ntype : " + type.ToString() + "\n### END ###\n";
        PushLog(log, false);
    }

    private float textAreaWidth = Screen.width * 0.7f;
    private float textAreaheight = Screen.height * 0.7f;
    private float space = Screen.height * 0.1f;
    private float fpsTimer = 0;
    private float fps = 0;
    private int btnDownTime = 3;
    private float btnDown = 0;
    private bool isDispLog = false;
    private float sizeRate = 1.0f;

    void OnGUI()
    {
        if (!AppManager.Instance.isDebug && !UserManager.isAdmin) return;

        sizeRate = ScreenManager.Instance.GetSizeRate();
        SetGuiSkin();
        
        //ログ
        Rect btnRect = new Rect(0, 0, space, space);
        Rect logRect = new Rect(0, space, textAreaWidth, textAreaheight);
        if (isDispLog)
        {
            //ログ表示中
            string logText = GetLogText(15000);
            GUI.TextArea(logRect, logText);
            if (GUI.Button(btnRect, "-"))
            {
                isDispLog = false;
                btnDown = 0;
            }
        }
        else
        {
            //ログ非表示中
            if (GUI.RepeatButton(btnRect, "", "button"))
            {
                btnDown += Time.deltaTime;
                if (btnDown >= btnDownTime)
                {
                    isDispLog = true;
                    GUIUtility.systemCopyBuffer = GetLogText();
                }
            }
        }

        //FPS
        fpsTimer += Time.deltaTime;
        if (fpsTimer >= 0.5f) {
            fpsTimer -= 0.5f;
            fps = Mathf.Round(10 / Time.deltaTime) / 10.0f;
        }
        float fpsW = 40;
        Rect fpsRect = new Rect(Screen.width - fpsW, 0, fpsW, 30);
        GUI.Label(fpsRect, fps.ToString());
    }
    private void SetGuiSkin(float sizeRate = 1)
    {
        GUI.skin.button.normal.background = null;
        GUI.skin.button.hover.background = null;
        GUI.skin.button.active.background = null;
        GUI.skin.label.fontSize = GetFixSize(36);
        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
        GUI.skin.textArea.fontSize = GetFixSize(36);
    }
    private float GetFixSize(float size)
    {
        return size * sizeRate;
    }
    private int GetFixSize(int size)
    {
        float sizef = GetFixSize((float)size);
        return Mathf.RoundToInt(sizef);
    }
    private string GetLogText(int limit = 0)
    {
        string logText = "";
        foreach (string log in logQueue)
        {
            logText = log + logText;
            if (limit > 0 && logText.Length >= limit) break;
        }
        return logText;
    }

    /**
     * @brief ログのプッシュ(エンキュー)
     * @param str プッシュするログ
     * @param console trueならばUnityのコンソール上にも表示する
     */
    public void PushLog(string str, bool console = true)
    {
        if (logQueue.Count >= logCount) logQueue.Dequeue();
        
        logQueue.Enqueue(str);
        if (console) Debug.Log(str);
    }
}