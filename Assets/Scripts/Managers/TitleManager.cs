using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    private Transform titleCanvasTran;
    private Text msgTxt; 

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        titleCanvasTran = GameObject.Find("TitleCanvas").transform;
        msgTxt = titleCanvasTran.Find("Message").GetComponent<Text>();
        msgTxt.gameObject.SetActive(false);
    }

    IEnumerator Start()
    {
        for (; ; )
        {
            if (AppManager.Instance.isReadyGame) break;
            yield return null;
        }
        for (; ;)
        {
            if (!ScreenManager.Instance.isSceneFade) break;
            yield return null;
        }

        //BgmManager.Instance.PlayBgm();
        //ScreenManager.Instance.OpenMessage("Tap to Start");
        msgTxt.text = "Tap to Start";
        msgTxt.gameObject.SetActive(true);

        //TapAction
        InputManager.Instance.SetTapAction(Demo);

        InputManager.Instance.SetActive(true);
    }

    private void Demo(InputStatus input)
    {
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_BATTLE);
    }

}