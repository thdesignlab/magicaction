using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MenuManager : SingletonMonoBehaviour<MenuManager>
{
    [SerializeField]
    private GameObject menu;

    private Transform myTran;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();
        menu.SetActive(false);
    }

    //##### 通常メニュー #####

    //メニュー
    public void SwitchMenu(bool flg)
    {
        menu.SetActive(flg);
    }

    //アプリ終了
    public void OnExitButton()
    {
        UnityAction callback = () => Application.Quit();

        DialogManager.Instance.OpenSelect("アプリを終了します。", callback);
    }

    //タイトルへ戻る
    public void OnTitleButton()
    {
        UnityAction callback = () => BattleManager.Instance.Return();

        DialogManager.Instance.OpenSelect("タイトルへ戻ります。", callback);
    }

    //ヘルプ
    public void OnHelpButton()
    {
        menu.transform.Find("Help").gameObject.SetActive(true);
    }
    public void OnHelpCloseButton()
    {
        menu.transform.Find("Help").gameObject.SetActive(false);
    }


    //コンフィグ
    public void OnConfigButton()
    {
        DialogManager.Instance.Open("Config!");
    }

}
