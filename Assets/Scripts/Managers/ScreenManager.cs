using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScreenManager : SingletonMonoBehaviour<ScreenManager>
{
    private Transform _commonCanvas;
    private Transform commonCanvas
    {
        get { return _commonCanvas ? _commonCanvas : _commonCanvas = GameObject.Find("CommonCanvas").transform; }
    }

    private Image fadeImg;
    private GameObject msgObj;
    private Image msgImg;
    private Text msgTxt;

    [SerializeField]
    private float fadeTime = 0.5f;

    [HideInInspector]
    public bool isSceneFade = false;
    [HideInInspector]
    public bool isUiFade = false;

    private float sizeRate = 1;
    private float targetRatio;
    private int preWidth;
    private int preHeight;

    private DeviceOrientation preOrientation;

    //メッセージ
    public const string MESSAGE_LOADING = "Now Loading...";

    private static string MESSAGE_IMAGE_LOADING = "nowloading";
    private static Dictionary<string, string> messageImgNameDic = new Dictionary<string, string>()
    {
    };
    private static Dictionary<string, Sprite> messageImgDic = null;

    protected override void OnInitialize()
    {
        DontDestroyOnLoad(commonCanvas);

        //フェード
        fadeImg = commonCanvas.Find("Fade").GetComponent<Image>();
        fadeImg.raycastTarget = false;
        fadeImg.color = new Color(fadeImg.color.r, fadeImg.color.g, fadeImg.color.b, 0);

        //メッセージ
        Transform msgTran = commonCanvas.Find("Message");
        msgObj = msgTran.gameObject;
        msgImg = msgTran.Find("Image").GetComponent<Image>();
        msgTxt = msgTran.Find("Text").GetComponent<Text>();

        //アス比
        targetRatio = Mathf.Round(Common.CO.SCREEN_WIDTH * 100 / Common.CO.SCREEN_HEIGHT);
        preWidth = Common.CO.SCREEN_WIDTH;
        preHeight = Common.CO.SCREEN_HEIGHT;

        //画面向き
        preOrientation = Input.deviceOrientation;

        //シーン遷移時イベント
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void Update()
    {
        if (Common.FUNC.IsPc())
        {
            //アス比チェック
            if (preWidth != Screen.width || preHeight != Screen.height)
            {
                AdjustScreen();
            }
        }

        //画面向きチェック
        SetOrientation();
    }

    //シーン遷移時イベント
    void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
    {
        commonCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        AdjustScreen();
    }

    public void SceneLoad(string sceneName, string message = MESSAGE_LOADING)
    {
        StartCoroutine(LoadProccess(sceneName, message));
    }

    IEnumerator LoadProccess(string sceneName, string message = "")
    {
        Image[] imgs = new Image[] { fadeImg };

        isSceneFade = true;

        //メッセージ表示
        OpenMessage();

        //フェードアウト
        yield return StartCoroutine(Fade(imgs, false));

        //シーンロード
        SceneManager.LoadScene(sceneName);

        //フェードイン
        yield return StartCoroutine(Fade(imgs, true));

        //メッセージ非表示
        CloseMessage();

        isSceneFade = false;
    }

    IEnumerator Fade(Image[] imgs, bool isFadeIn, bool isBlackOut = true)
    {
        if (imgs.Length == 0 || fadeTime <= 0) yield break;

        Color invisibleColor = (isBlackOut) ? new Color(0, 0, 0, 0) : new Color(1, 1, 1, 1);
        Color visibleColor = (isBlackOut) ? new Color(0, 0, 0, 1) : new Color(1, 1, 1, 0);
        float procTime = 0;
        for (;;)
        {
            procTime += Time.unscaledDeltaTime;
            float procRate = procTime / fadeTime;
            Color startColor;
            Color endColor;
            startColor = (isFadeIn) ? visibleColor : invisibleColor;
            endColor = (isFadeIn) ? invisibleColor : visibleColor;
            foreach (Image img in imgs)
            {
                img.enabled = true;
                img.raycastTarget = isBlackOut;
                img.color = Color.Lerp(startColor, endColor, procRate);
                if (procRate >= 1)
                {
                    img.raycastTarget = !isBlackOut;
                    img.enabled = false;
                }
            }
            if (procRate >= 1) break;
            yield return null;
        }
    }

    public IEnumerator VerticalFade(Transform t, bool isFadeIn, UnityAction callback = null)
    {
        Vector3 baseScale = t.localScale;
        Vector3 fadeScale = new Vector3(baseScale.x, 0, baseScale.z);
        Vector3 startScale = (isFadeIn) ? fadeScale : baseScale;
        Vector3 endScale = (isFadeIn) ? baseScale : fadeScale;
        float rate = 0;
        float fadeTime = 0.1f;
        for (;;)
        {
            if (t == null) break;
            rate += Time.unscaledDeltaTime / fadeTime;
            t.localScale = Vector3.Lerp(startScale, endScale, rate);
            if (rate >= 1) break;
            yield return null;
        }
        if (callback != null) callback.Invoke();
    }

    //public void FadeUI(GameObject fadeOutObj, GameObject fadeInObj, bool isChild = true)
    //{
    //    StartCoroutine(LoadUIProccess(fadeOutObj, fadeInObj, isChild));
    //}

    //public void FadeUI(GameObject uiObj, bool isFadeIn, bool isChild = true)
    //{
    //    GameObject fadeOutObj = null;
    //    GameObject fadeInObj = null;
    //    if (isFadeIn)
    //    {
    //        fadeInObj = uiObj;
    //    }
    //    else
    //    {
    //        fadeOutObj = uiObj;
    //    }
    //    StartCoroutine(LoadUIProccess(fadeOutObj, fadeInObj, isChild));
    //}

    //IEnumerator LoadUIProccess(GameObject fadeOutObj, GameObject fadeInObj, bool isChild)
    //{
    //    if (isUiFade)
    //    {
    //        for (;;)
    //        {
    //            if (!isUiFade) break;
    //            yield return null;
    //        }
    //    }

    //    isUiFade = true;
    //    //if (isLoadMessage) DialogController.OpenMessage(DialogController.MESSAGE_LOADING, DialogController.MESSAGE_POSITION_RIGHT);

    //    if (fadeOutObj != null)
    //    {
    //        //フェードアウト
    //        Image[] fadeOutImgs;
    //        if (isChild)
    //        {
    //            fadeOutImgs = GetComponentsInChildrenWithoutSelf<Image>(fadeOutObj.transform);
    //        }
    //        else
    //        {
    //            fadeOutImgs = fadeOutObj.transform.GetComponents<Image>();
    //        }
    //        Coroutine fadeOut = StartCoroutine(Fade(fadeOutImgs, false, false));
    //        yield return fadeOut;
    //        fadeOutObj.SetActive(false);
    //    }

    //    if (fadeInObj != null)
    //    {
    //        //フェードイン
    //        Image[] fadeInImgs;
    //        if (isChild)
    //        {
    //            fadeInImgs = GetComponentsInChildrenWithoutSelf<Image>(fadeInObj.transform);
    //        }
    //        else
    //        {
    //            fadeInImgs = fadeInObj.transform.GetComponents<Image>();
    //        }
    //        Coroutine fadeIn = StartCoroutine(Fade(fadeInImgs, true, false));
    //        fadeInObj.SetActive(true);
    //        yield return fadeIn;
    //    }

    //    //if (isLoadMessage) DialogController.CloseMessage();
    //    isUiFade = false;
    //}

    //public static T[] GetComponentsInChildrenWithoutSelf<T>(Transform self)
    //{
    //    List<T> compList = new List<T>(); 
    //    foreach (Transform child in self)
    //    {
    //        T comp = child.GetComponent<T>();
    //        if (comp != null) compList.Add(comp);
    //    }
    //    return compList.ToArray();
    //}

    //private bool IsFadeImage(Image img)
    //{
    //    if (img == fadeImg) return true;
    //    if (img.sprite == null) return false;

    //    switch (img.sprite.name)
    //    {
    //        case "Background":
    //            return false;
    //    }

    //    return true;
    //}

    //public void TextFadeOut(Text obj, float time = -1)
    //{
    //    if (time <= 0) time = fadeTime;
    //    StartCoroutine(TextFadeOutProc(obj, time));
    //}
    //IEnumerator TextFadeOutProc(Text obj, float time)
    //{
    //    float startAlpha = obj.color.a;
    //    float nowAlpha = startAlpha;
    //    for (;;)
    //    {
    //        nowAlpha -= Time.deltaTime / time * startAlpha;
    //        obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, nowAlpha);
    //        if (nowAlpha <= 0) break;
    //        yield return null;
    //    }
    //}

    //public void ImageFadeOut(Image obj, float time = -1)
    //{
    //    if (time <= 0) time = fadeTime;
    //    StartCoroutine(ImageFadeOutProc(obj, time));
    //}
    //IEnumerator ImageFadeOutProc(Image obj, float time)
    //{
    //    float startAlpha = obj.color.a;
    //    float nowAlpha = startAlpha;
    //    for (;;)
    //    {
    //        nowAlpha -= Time.deltaTime / time * startAlpha;
    //        obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, nowAlpha);
    //        if (nowAlpha <= 0) break;
    //        yield return null;
    //    }
    //}

    public void OpenMessage(string text = MESSAGE_LOADING)
    {
        if (text == "")
        {
            CloseMessage();
            return;
        }

        if (msgTxt.text == text) return;

        Sprite image = GetMessageImage(text);
        msgTxt.text = text;
        if (image != null)
        {
            //画像
            msgImg.sprite = image;
            msgImg.enabled = true;
            msgTxt.enabled = false;
        }
        else
        {
            //テキスト
            msgImg.sprite = null;
            msgImg.enabled = false;
            msgTxt.enabled = true;
        }
        msgObj.SetActive(true);
    }

    public void CloseMessage()
    {
        msgImg.sprite = null;
        msgImg.enabled = false;
        msgTxt.text = "";
        msgTxt.enabled = false;
        msgObj.SetActive(false);
    }

    private static Sprite GetMessageImage(string text)
    {
        //if (messageImgDic == null)
        //{
        //    Sprite[] messageImgs = Resources.LoadAll<Sprite>(Common.CO.RESOURCE_MESSAGE_DIR);
        //    messageImgDic = new Dictionary<string, Sprite>();
        //    foreach (Sprite img in messageImgs)
        //    {
        //        messageImgDic.Add(img.name, img);
        //    }
        //}

        Sprite image = null;
        if (messageImgNameDic.ContainsKey(text))
        {
            string imageName = messageImgNameDic[text];
            if (messageImgDic.ContainsKey(imageName))
            {
                image = messageImgDic[imageName];
            }
        }
        return image;
    }

    //### アスペクト比固定 ###

    //UIを調整
    private void AdjustScreen()
    {
        preWidth = Screen.width;
        preHeight = Screen.height;
        float nowRatio = (float)Mathf.Round(100 * Screen.width / Screen.height);
        float x = 0;
        float y = 0;
        float w = 1;
        float h = 1;

        if (targetRatio > nowRatio)
        {
            //横に合わせる
            sizeRate = (float)Screen.width / Common.CO.SCREEN_WIDTH;
            float targetL = Common.CO.SCREEN_HEIGHT * sizeRate;
            float r = (Screen.height - targetL) / Screen.height;
            y = r / 2;
            h -= r;
        }
        else if (targetRatio < nowRatio)
        {
            //縦に合わせる
            sizeRate = (float)Screen.height / Common.CO.SCREEN_HEIGHT;
            float targetL = Common.CO.SCREEN_WIDTH * sizeRate;
            float r = (Screen.width - targetL) / Screen.width;
            x = r / 2;
            w -= r;
        }
        else
        {
            sizeRate = 1.0f;
        }

        Camera.main.rect = new Rect(x, y, w, h);
    }

    public float GetSizeRate()
    {
        return sizeRate;
    }

    //###　画面向き ###

    private void SetOrientation()
    {
        DeviceOrientation nowOrientation = Input.deviceOrientation;
        if (preOrientation == nowOrientation) return;

        ScreenOrientation screen = Screen.orientation;
        preOrientation = nowOrientation;

        switch (nowOrientation)
        {
            case DeviceOrientation.LandscapeLeft:
                screen = ScreenOrientation.LandscapeLeft;
                break;

            case DeviceOrientation.LandscapeRight:
                screen = ScreenOrientation.LandscapeRight;
                break;

            case DeviceOrientation.Portrait:
            case DeviceOrientation.PortraitUpsideDown:
                if (Input.acceleration.x > 0)
                {
                    screen = ScreenOrientation.LandscapeRight;
                }
                else
                {
                    screen = ScreenOrientation.LandscapeLeft;
                }
                break;

            default:
                screen = ScreenOrientation.Landscape;
                break;
        }
        Screen.orientation = screen;
    }
}

