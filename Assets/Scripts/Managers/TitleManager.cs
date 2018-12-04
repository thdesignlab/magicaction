using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class TitleManager : SingletonMonoBehaviour<TitleManager>
{
    private Transform titleCanvasTran;
    private Text msgTxt;
    private GameObject stageList;
    private Transform stageContentTran;

    const string STAGE_PREFIX = "STAGE";

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        titleCanvasTran = GameObject.Find("TitleCanvas").transform;
        msgTxt = titleCanvasTran.Find("Message").GetComponent<Text>();
        msgTxt.gameObject.SetActive(false);

        //ステージリスト
        Transform stageListTran = titleCanvasTran.Find("StageList");
        stageList = stageListTran.gameObject;
        stageContentTran = stageListTran.Find("Viewport/Content");
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

        msgTxt.text = "Now Loading";
        msgTxt.gameObject.SetActive(true);

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
        GameObject stageObj = Resources.Load<GameObject>("UIs/Stage");
        Sprite activeStar = Resources.Load<Sprite>("Sprites/UI_ster_on");
        for (int i = 1; i <= 10; i++)
        {
            int stageNo = i;
            GameObject obj = Instantiate(stageObj, Vector2.zero, Quaternion.identity);
            Button btn = obj.GetComponent<Button>();
            obj.transform.Find("No").GetComponent<Text>().text = STAGE_PREFIX + i.ToString();
            obj.transform.SetParent(stageContentTran, false);
            if (stageNo <= 5)
            {
                obj.GetComponent<Button>().interactable = true;
                int temp = stageNo % 4;
                for (int j = 1; j <= temp; j++)
                {
                    obj.transform.Find("Stars/Star" + j.ToString()).GetComponent<Image>().sprite = activeStar;
                }
                btn.onClick.AddListener(() => AppManager.Instance.StageSelect(stageNo));
            }
            else
            {
                btn.interactable = false;
            }

        }

        AppManager.Instance.isOnTapToStart = true;
        msgTxt.gameObject.SetActive(false);
        stageList.SetActive(true);
    }

}