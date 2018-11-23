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
        //DialogController.OpenDialog("アプリを終了します", () => GameController.Instance.Exit(), true);
    }

    //タイトルへ戻る
    public void OnTitleButton()
    {
        UnityAction callback = () =>
        {
            BattleManager.Instance.Return();
        };

        //DialogController.OpenDialog("タイトルに戻ります", callback, true);
    }

    //一時停止
    public void OnPauseButton()
    {
        //GameController.Instance.Pause();
    }

}
