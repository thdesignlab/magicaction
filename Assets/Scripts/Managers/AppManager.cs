using UnityEngine;
using UnityEngine.Events;
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

    public void StageSelect(int no)
    {
        GameObject stage = Resources.Load<GameObject>("Timelines/Stage" + no.ToString());
        if (stage == null)
        {
            if (isDebug)
            {
                no = 0;
                stage = Resources.Load<GameObject>("Timelines/StageExtra");
            }
            else
            {
                //error
                ErrorStageSelect();
                return;
            }
        }
        string sceneName = stage.GetComponent<StageManager>().GetStageScene();
        if (string.IsNullOrEmpty(sceneName))
        {
            //error
            ErrorStageSelect();
            return;
        }
        stageNo = no;
        stageObj = stage;
        ScreenManager.Instance.SceneLoad(sceneName);
    }
    public void NextStage()
    {
        StageSelect(stageNo + 1);
    }

    private void ErrorStageSelect()
    {
        UnityAction callback = () => ScreenManager.Instance.SceneLoad(Common.CO.SCENE_TITLE);
        DialogManager.Instance.OpenMessage("ステージ情報取得に失敗しました。", callback);
    }
}