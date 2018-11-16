using UnityEngine;
using System.Collections;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();
    }

    IEnumerator Start()
    {
        for (; ; )
        {
            if (AppManager.Instance.isReadyGame) break;
            yield return null;
        }

        //BgmManager.Instance.PlayBgm();
    }

    public void Demo()
    {
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_BATTLE);
    }
}