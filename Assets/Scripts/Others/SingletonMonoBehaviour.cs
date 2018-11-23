using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static bool isInit = true;

    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));

                if (instance == null)
                {
                    Debug.LogError(typeof(T) + "is nothing");
                }
            }

            return instance;
        }
    }

    static void Initialize(T paramInstance)
    {
        if (instance == null)
        {
            instance = paramInstance;
        }
        else if (instance != paramInstance)
        {
            isInit = false;
            Destroy(paramInstance.gameObject);
        }
    }

    protected bool isDontDestroyOnLoad = true;

    protected virtual void Awake()
    {
        Initialize(this as T);
        if (isDontDestroyOnLoad) DontDestroyOnLoad(gameObject);
        if (isInit) OnInitialize();
    }

    protected virtual void OnInitialize()
    {

    }
}