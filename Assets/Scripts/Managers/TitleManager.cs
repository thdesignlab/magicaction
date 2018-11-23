﻿using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    protected override void Awake()
    {
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
        ScreenManager.Instance.OpenMessage("Tap to Start");

        //TapAction
        InputManager.Instance.SetTapAction(Demo);

        InputManager.Instance.SetActive(true);
    }

    private void Demo(InputStatus input)
    {
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_BATTLE);
    }
}