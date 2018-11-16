using UnityEngine;
using System.Collections;

public class AppManager : SingletonMonoBehaviour<AppManager>
{
    [SerializeField]
    private bool isDebugMode;
    public static bool isDebug;

    [HideInInspector]
    public bool isReadyGame = false;
    [HideInInspector]
    public bool isFinishedSplash = false;

    public static ScreenManager screenManager;

    protected override void Awake()
    {
        base.Awake();

        Debug.Log("isDebugMode=" + isDebugMode);
        isDebug = isDebugMode;
        screenManager = GetComponent<ScreenManager>();
    }

    IEnumerator Start()
    {
#if UNITY_EDITOR
#elif UNITY_IOS || UNITY_ANDROID
        //スプラッシュ終了待ち
        for (;;)
        {
            if (UnityEngine.Rendering.SplashScreen.isFinished) break;
            yield return null;
        }
#else
        
#endif
        isFinishedSplash = true;

        //フレームレート
        Application.targetFrameRate = 60;

        //BGM
        BgmManager.Instance.Init();

        isReadyGame = true;
        yield return null;
    }
}