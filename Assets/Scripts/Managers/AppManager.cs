using UnityEngine;
using System.Collections;

public class AppManager : SingletonMonoBehaviour<AppManager>
{
    [SerializeField]
    private bool isDebugMode;
    public static bool isDebug;

    public static ScreenManager screenManager;

    private void Start()
    {
        Debug.Log("isDebugMode=" + isDebugMode);
        isDebug = isDebugMode;
        screenManager = this.gameObject.GetComponent<ScreenManager>();
    }
}