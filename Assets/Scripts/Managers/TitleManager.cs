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

        //ステージリストセット
        Transform stageListTran = titleCanvasTran.Find("StageList");
        stageList = stageListTran.gameObject;
        Transform stageContentTran = stageListTran.Find("Viewport/Content");
        GameObject stageObj = Resources.Load<GameObject>("UIs/Stage");
        for (int i = 1; i < 20; i++)
        {
            int stageNo = i;
            GameObject obj = Instantiate(stageObj, Vector2.zero, Quaternion.identity);
            Button btn = obj.GetComponent<Button>();
            obj.transform.Find("No").GetComponent<Text>().text = STAGE_PREFIX+i.ToString();
            obj.transform.Find("Star").GetComponent<Text>().text = "☆☆☆";
            btn.onClick.AddListener(() => StageSelect(stageNo));
            //btn.interactable = false;
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

    private void StageSelect(int stageNo)
    {
        GameObject stage = Resources.Load<GameObject>("Timelines/Stage" + stageNo.ToString());
        if (stage == null)
        {
            if (AppManager.Instance.isDebug)
            {
                stageNo = 0;
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
        AppManager.Instance.stageNo = stageNo;
        AppManager.Instance.stageObj = stage;
        ScreenManager.Instance.SceneLoad(sceneName);
    }

    private void ErrorStageSelect()
    {
        UnityAction callback = () => ScreenManager.Instance.SceneLoad(Common.CO.SCENE_TITLE);
        DialogManager.Instance.OpenMessage("ステージ情報取得に失敗しました。", callback);
    }
}