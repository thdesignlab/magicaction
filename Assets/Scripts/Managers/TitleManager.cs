using UnityEngine;
using System.Collections;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();
    }


    public void Demo()
    {
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_BATTLE);
    }
}