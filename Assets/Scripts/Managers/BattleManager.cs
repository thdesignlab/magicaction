using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    private int stageNo;
    private float battleEndProgress = 0;
    private int battleLoopCnt = 0;
    private Dictionary<int, GameObject> popEnemy = new Dictionary<int, GameObject>();
    private Dictionary<int, float> popInterval = new Dictionary<int, float>();
    private Dictionary<int, float> popTime = new Dictionary<int, float>();
    private GameObject[] enemyPops;
    private GameObject[] enemyGroundPops;

    //キャンバス
    private Canvas battleCanvas;

    //プレイヤー情報
    private Slider hpSlider;
    private Slider hpAlphaSlider;
    private Slider mpSlider;
    private Text totalText;
    private Text killText;
    private Text lostText;
    private Text hitText;

    //Stage進捗
    private Slider progressSlider;
    private Text progressStartText;
    private Text progressEndText;

    //メッセージ
    private Text messageText;

    //リザルト
    private GameObject resultArea;
    private Dictionary<string, List<WeaponResult>> weaponResults = new Dictionary<string, List<WeaponResult>>();

    private bool isBattleStart = false;
    private bool isBattleEnd = false;
    private int totalScore = 0;
    private int killScore = 0;
    private int lostScore = 0;
    private int hitScore = 0;
    private PlayableDirector timeline;
    private PlayerController playerCtrl;
    private bool isPause = false;
    private bool isEndless = false;
    private int loopCount = 0;
    private float powRate = 1.0f;
    private float mpBonus = 0;
    private float mpBonusCheckTime = 0;
    private Dictionary<float, float> scoreBonusByMpDic = new Dictionary<float, float>()
    {
        { 0.75f, 1.0f},
        { 0.5f, 1.1f},
        { 0.25f, 1.25f},
        { 0.1f, 1.5f},
        { 0, 1.8f},
    };

    private Dictionary<string, EnemyBossController> bossDic = new Dictionary<string, EnemyBossController>();

    const int KILL_POINT = 100;
    const float MP_BONUS_CHECK_TIME = 0.5f;
    const string PROGRESS_START = "Start";
    const string PROGRESS_END = "End";
    const string PROGRESS_WAVE = "Wave";

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        stageNo = AppManager.Instance.stageNo;
        battleCanvas = GameObject.Find("BattleCanvas").GetComponent<Canvas>();
        battleCanvas.worldCamera = Camera.main;
        hpSlider = battleCanvas.transform.Find("BattleStatus/PlayerStatus/HP").GetComponent<Slider>();
        hpSlider.value = 0;
        mpSlider = battleCanvas.transform.Find("BattleStatus/PlayerStatus/MP").GetComponent<Slider>();
        mpSlider.value = 0;
        Transform battleStatusTran = battleCanvas.transform.Find("BattleStatus");
        totalText = battleStatusTran.Find("Scores/Total/Score").GetComponent<Text>();
        killText = battleStatusTran.Find("Scores/Kill/Score").GetComponent<Text>();
        lostText = battleStatusTran.Find("Scores/Lost/Score").GetComponent<Text>();
        //hitText = battleStatusTran.Find("Scores/Hit/Score").GetComponent<Text>();
        SetScoreText();
        progressSlider = battleStatusTran.Find("Progress/Slider").GetComponent<Slider>();
        progressStartText = battleStatusTran.Find("Progress/Texts/Start").GetComponent<Text>();
        progressEndText = battleStatusTran.Find("Progress/Texts/End").GetComponent<Text>();
        progressSlider.value = 0;
        messageText = battleCanvas.transform.Find("Message").GetComponent<Text>();
        resultArea = battleCanvas.transform.Find("Result").gameObject;
        resultArea.SetActive(false);
        mpBonusCheckTime = MP_BONUS_CHECK_TIME;
        SetStageTimeline();
    }

    private void Start()
    {
        StartCoroutine(BattleStart());
    }

    private void Update()
    {
        if (!isBattleStart || isBattleEnd) return;

        SetProgressSlider();
    }

    public int GetEnemyCount()
    {
        int cnt = 0;
        foreach (string tag in Common.CO.enemyTags)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
            if (enemies != null) cnt += enemies.Length;
        }
        return cnt;
    }

    IEnumerator SetStatusSlider()
    {
        float hpUnit = 3.0f;
        float mpUnit = 3.0f;
        float hpRate = 0;
        float mpRate = 0;
        float diffHpRate = 0;
        float diffMpRate = 0;
        float preHpRate = 0;
        float preMpRate = 0;
        for (; ; )
        {
            if (isPause)
            {
                yield return null;
                continue;
            }
            float deltaTime = Time.deltaTime;
            mpBonusCheckTime -= deltaTime;

            if (playerCtrl == null || !isBattleStart)
            {
                hpRate = 1;
                mpRate = 1;
                if (isBattleStart)
                {
                    if (preHpRate <= 0) break;
                    hpRate = 0;
                    mpRate = 0;
                }
            }
            else
            {
                hpRate = playerCtrl.GetHpRate();
                mpRate = playerCtrl.GetMpRate();
            }
            diffHpRate = hpRate - preHpRate;
            diffMpRate = mpRate - preMpRate;

            //HP
            if (diffHpRate != 0)
            {
                diffHpRate = hpUnit * Mathf.Sign(diffHpRate) * deltaTime;
                if (Mathf.Abs(hpRate - hpSlider.value) > Mathf.Abs(diffHpRate))
                {
                    hpSlider.value += diffHpRate;
                }
                else
                {
                    hpSlider.value = hpRate;
                }
                preHpRate = hpSlider.value;
            }

            //MP
            if (diffMpRate != 0)
            {
                diffMpRate = mpUnit * Mathf.Sign(diffMpRate) * deltaTime;
                if (Mathf.Abs(mpRate - mpSlider.value) > Mathf.Abs(diffMpRate))
                {
                    mpSlider.value += diffMpRate;
                }
                else
                {
                    mpSlider.value = mpRate;
                }
                preMpRate = mpSlider.value;
                if (mpBonusCheckTime <= 0)
                {
                    SetMpRateBonus();
                    mpBonusCheckTime = MP_BONUS_CHECK_TIME;
                }
            }
            yield return new WaitForSeconds(deltaTime);
        }
    }
    private float SliderRate(float f)
    {
        return (float)Mathf.Floor(f * 1000) / 1000;
    }

    //Timelineセット
    private void SetStageTimeline()
    {
        if (AppManager.Instance.stageObj == null)
        {
            Pause();
            UnityAction callback = () => Return();
            DialogManager.Instance.OpenMessage("ステージ情報取得に失敗しました。", callback);
            return;
        }
        GameObject obj = Instantiate(AppManager.Instance.stageObj);
        timeline = obj.GetComponent<PlayableDirector>();
        timeline.playOnAwake = false;

        //プログレスバー
        string startTxt = PROGRESS_START;
        string endTxt = PROGRESS_END;
        TrackAsset[] tracks = (timeline.playableAsset as TimelineAsset).GetOutputTracks() as TrackAsset[];
        foreach (TrackAsset track in tracks)
        {
            if (StageManager.Instance.IsBoss())
            {
                //ボス戦の場合はHP
                IEnumerable<TimelineClip> clips = track.GetClips();
                TimelineClip clip = clips.FirstOrDefault(x => x.asset is EnemyBossAsset);
                if (clip == null) continue;
                EnemyBossAsset eba = (EnemyBossAsset)clip.asset;
                battleEndProgress += eba.GetBossHp();
            }
            else
            {
                //通常戦の場合はタイムライン終了時間
                if (battleEndProgress > track.end) continue;
                battleEndProgress = (float)track.end;
            }
        }
        SetPowRate();
        progressStartText.text = startTxt;
        progressEndText.text = endTxt;
    }
    public void TimelineLoop()
    {
        if (timeline.state == PlayState.Playing || isBattleEnd) return;
        if (!StageManager.Instance.IsBoss())
        {
            battleLoopCnt++;
            SetPowRate();
            progressEndText.text = PROGRESS_WAVE + (battleLoopCnt + 1).ToString();
        }
        timeline.Play();
    }
    private void SetPowRate()
    {
        float rate = StageManager.Instance.GetPowoerUpRate();
        if (StageManager.Instance.IsEndless())
        {
            rate *= battleLoopCnt;
        }
        powRate = 1 + rate / 100;
    }
    private void SetProgressSlider()
    {
        float value = 0;
        if (timeline.state == PlayState.Playing)
        {
            if (StageManager.Instance.IsBoss())
            {
                int totalDamage = 0;
                foreach (string bossName in bossDic.Keys)
                {
                    totalDamage += bossDic[bossName].GetDamage();
                }
                value = totalDamage / battleEndProgress;
            }
            else
            {
                value = (float)timeline.time / battleEndProgress;
            }
        }
        else if (isBattleStart)
        {
            value = 1;
        }
        progressSlider.value = value;
    }

    IEnumerator BattleStart()
    {
        for (; ; )
        {
            if (!ScreenManager.Instance.isSceneFade) break;
            yield return null;
        }
        SetMessage("READY...");

        //プレイヤー生成
        StartCoroutine(SetStatusSlider());
        yield return StartCoroutine(PlayerSummon());

        InputManager.Instance.SetActive(true);
        SetMessage("START", 3.0f);

        isBattleStart = true;
        timeline.Play();

        StartCoroutine(CheckBattleResult());
    }

    IEnumerator PlayerSummon()
    {
        GameObject player = Resources.Load<GameObject>("Players/Player");
        GameObject summon = Resources.Load<GameObject>("Directions/SummonPlayer");
        Transform pPopTran = GameObject.FindGameObjectWithTag(Common.CO.TAG_PLAYER_POP).transform;

        //召喚演出
        GameObject summonObj = Instantiate(summon, pPopTran.position - Vector3.up * 0.5f, pPopTran.rotation);
        yield return new WaitForSeconds(0.5f);
        GameObject playerObj = Instantiate(player, pPopTran.position + Vector3.up * 1.5f, pPopTran.rotation);
        playerCtrl = playerObj.GetComponent<PlayerController>();
        yield return new WaitForSeconds(2.5f);
        Destroy(summonObj);
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator CheckBattleResult()
    {
        for (; ; )
        {
            if (isPause)
            {
                yield return null;
                continue;
            }

            if (isBattleStart && !isBattleEnd)
            {
                if (playerCtrl == null)
                {
                    StartCoroutine(Result(false));
                    break;
                }
                if (StageManager.Instance.IsBoss())
                {
                    if (progressSlider.value == 1)
                    {
                        StartCoroutine(Result(true));
                        break;
                    }
                }
                else if (timeline.state == PlayState.Paused)
                {
                    if (GetEnemyCount() == 0)
                    {
                        StartCoroutine(Result(true));
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    IEnumerator Result(bool isClear)
    {
        InputManager.Instance.SetActive(false);
        timeline.Stop();
        isBattleEnd = true;
        Transform resultTran = resultArea.transform;
        resultTran.Find("Text").GetComponent<Text>().text = isClear ? "Clear!" : "Failed!";
        resultTran.Find("BtnList/Next").gameObject.SetActive(isClear);
        resultTran.Find("BtnList/Retry").gameObject.SetActive(!isClear);

        //resultTran.Find("Score").GetComponent<Text>().text = "";
        resultArea.SetActive(true);
        yield return null;
    }

    public GameObject SqawnEnemy(GameObject obj, Vector2 pos, Quaternion qua)
    {
        if (obj == null) return null;
        return Instantiate(obj, pos, qua);
    }

    public void SqawnEnemyBoss(GameObject obj, Vector2 pos, Quaternion qua)
    {
        if (obj == null) return;
        if (bossDic.ContainsKey(obj.name)) return;

        //召喚演出
        GameObject bossObj = SqawnEnemy(obj, pos, qua);
        EnemyBossController bossCtrl = bossObj.GetComponent<EnemyBossController>();
        bossDic.Add(obj.name, bossCtrl);
    }

    public void AddKill(float bonus = 0)
    {
        if (isBattleEnd) return;
        killScore += 1;
        totalScore += Mathf.FloorToInt(KILL_POINT * (1 + bonus + mpBonus));
        SetScoreText();
    }

    private void SetMpRateBonus()
    {
        if (playerCtrl == null) return;

        float mpRate = playerCtrl.GetMpRate();
        foreach (float r in scoreBonusByMpDic.Keys)
        {
            if (mpRate <= r) continue;
            mpBonus = scoreBonusByMpDic[r];
            break;
        }
    }

    public void AddLost()
    {
        if (isBattleEnd) return;
        lostScore += 1;
        SetScoreText();
    }
    public void AddHit()
    {
        if (isBattleEnd) return;
        hitScore += 1;
        SetScoreText();
    }

    protected void SetScoreText()
    {
        if (totalScore < 0) totalScore = 0;
        totalText.text = totalScore.ToString();
        killText.text = killScore.ToString();
        lostText.text = lostScore.ToString();
        //hitText.text = hitScore.ToString();
    }

    public void SetMessage(string txt, float limit = 0)
    {
        if (limit <= 0)
        {
            messageText.text = txt;
        }
        else
        {
            StartCoroutine(SetMessageProcess(txt, limit));
        }
    }
    IEnumerator SetMessageProcess(string txt, float limit)
    {
        messageText.text = txt;
        yield return new WaitForSeconds(limit);
        messageText.text = "";
    }

    public void NextStage()
    {
        Return();
    }

    public void Retry()
    {
        ScreenManager.Instance.SceneLoad(SceneManager.GetActiveScene().name);
    }

    //### WeaponResult ###


    //### getter/setter ###

    public float GetPowRate()
    {
        return powRate;
    }

    public bool IsBattleEnd()
    {
        return isBattleEnd;
    }

    //### イベント ###

    void OnApplicationPause(bool flg)
    {
        if (isBattleEnd) return;

        if (flg)
        {
            MenuManager.Instance.SwitchMenu(true);
        }
    }

    //### MENU ###

    public void Pause()
    {
        Time.timeScale = 0;
        isPause = true;
    }
    public void ResetPause()
    {
        Time.timeScale = 1;
        isPause = false;
    }

    public void Return()
    {
        InputManager.Instance.SetActive(false);
        if (timeline) timeline.Stop();
        Time.timeScale = 1;
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_TITLE);
    }
}