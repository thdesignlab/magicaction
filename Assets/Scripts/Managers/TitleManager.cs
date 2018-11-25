using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    private Transform titleCanvasTran;
    private Text msgTxt;
    private GameObject stageList;

    const string STAGE_PREFIX = "STAGE";

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        titleCanvasTran = GameObject.Find("TitleCanvas").transform;
        msgTxt = titleCanvasTran.Find("Message").GetComponent<Text>();
        msgTxt.gameObject.SetActive(false);

        Transform stageListTran = titleCanvasTran.Find("StageList");
        stageList = stageListTran.gameObject;
        Transform stageContentTran = stageListTran.Find("Viewport/Content");
        GameObject stageObj = Resources.Load<GameObject>("UIs/Stage");
        for (int i = 1; i < 20; i++)
        {
            GameObject obj = Instantiate(stageObj, Vector2.zero, Quaternion.identity);
            Button btn = obj.GetComponent<Button>();
            obj.transform.Find("No").GetComponent<Text>().text = STAGE_PREFIX+i.ToString();
            obj.transform.Find("Star").GetComponent<Text>().text = "☆☆☆";
            if (i == 1)
            {
                btn.onClick.AddListener(() => Demo());
            }
            else if (i == 2)
            {
                btn.onClick.AddListener(() => Demo2());
            }
            else
            {
                btn.interactable = false;
            }
            obj.transform.SetParent(stageContentTran, false);
        }
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

        if (!AppManager.Instance.isOnTapToStart)
        {
            msgTxt.text = "Tap to Start";
            msgTxt.gameObject.SetActive(true);

            //TapAction
            InputManager.Instance.SetTapAction(DispStageList);
        }
        else
        {
            DispStageList(null);
        }


        InputManager.Instance.SetActive(true);
    }

    private void DispStageList(InputStatus input)
    {
        AppManager.Instance.isOnTapToStart = true;
        msgTxt.gameObject.SetActive(false);
        stageList.SetActive(true);
    }
    private void Demo()
    {
        AppManager.Instance.stageNo = 1;
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_BATTLE);
    }
    private void Demo2()
    {
        AppManager.Instance.stageNo = 2;
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_BATTLE_LARGE);
    }

}