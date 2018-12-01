using UnityEngine;
using System.Collections;

public class AppManager : SingletonMonoBehaviour<AppManager>
{
    public bool isDebug;

    [HideInInspector]
    public bool isFinishedSplash = false;
    [HideInInspector]
    public bool isReadyGame = false;
    [HideInInspector]
    public bool isOnTapToStart = false;

    [HideInInspector]
    public int stageNo;
    [HideInInspector]
    public GameObject stageObj;

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