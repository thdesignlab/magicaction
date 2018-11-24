using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogManager : SingletonMonoBehaviour<DialogManager>
{
    [SerializeField]
    private GameObject commonDialog;

    private Transform commonCanvasTran;
    private GameObject background;

    //ダイアログ
    private List<GameObject> openDialogList;

    //テキスト
    const string TEXT_DECISION = "OK";
    const string TEXT_CANCEL = "Cancel";

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        commonCanvasTran = GameObject.Find("CommonCanvas").transform;
        background = commonCanvasTran.Find("DialogBg").gameObject;
        background.SetActive(false);
    }

    //メッセージダイアログ
    public void OpenSelect(string message, UnityAction decisionAction, string decisionText = TEXT_DECISION)
    {
        Open(message, "", decisionText, decisionAction, true);
    }

    //選択ダイアログ

    public void Open (
        string message,
        string title = "",
        string decisionText = TEXT_DECISION,
        UnityAction decisionAction = null,
        bool isCancel = false,
        string cancelText = TEXT_CANCEL,
        UnityAction cancelAction = null,
        GameObject dialog = null
    ) {
        background.SetActive(true);

        //ダイアログ生成
        if (dialog == null) dialog = commonDialog;
        GameObject dialogObj = Instantiate(dialog, Vector2.zero, Quaternion.identity);
        Transform dialogTran = dialogObj.transform;
        dialogTran.SetParent(commonCanvasTran, false);

        //タイトル
        Transform titleTran = dialogTran.Find("Title");
        if (!string.IsNullOrEmpty(title))
        {
            titleTran.gameObject.SetActive(true);
            titleTran.GetComponent<Text>().text = title;
        }
        else
        {
            titleTran.gameObject.SetActive(false);
        }

        //メッセージ
        dialogTran.Find("Message").GetComponent<Text>().text = message;

        //決定ボタン
        Transform decisionTran = dialogTran.Find("Buttons/Decision");
        decisionTran.GetComponent<Button>().onClick.AddListener(() => OnClickButton(dialogObj, decisionAction));
        decisionTran.Find("Text").GetComponent<Text>().text = decisionText;

        //キャンセルボタン
        Transform cancelTran = dialogTran.Find("Buttons/Cancel");
        if (isCancel || cancelAction != null)
        {
            cancelTran.gameObject.SetActive(true);
            cancelTran.GetComponent<Button>().onClick.AddListener(() => OnClickButton(dialogObj, cancelAction));
            cancelTran.Find("Text").GetComponent<Text>().text = cancelText;
        }
        else
        {
            cancelTran.gameObject.SetActive(false);
        }
    }

    private void OnClickButton(GameObject dialog, UnityAction unityAction = null)
    {
        if (unityAction != null) unityAction.Invoke();
        Close(dialog);
    }

    private void Close(GameObject dialog)
    {
        Destroy(dialog);
        background.SetActive(false);
    }
}
