using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    private int stageNo;
    private float battleTime = 0;
    private Dictionary<int, GameObject> popEnemy = new Dictionary<int, GameObject>();
    private Dictionary<int, float> popInterval = new Dictionary<int, float>();
    private Dictionary<int, float> popTime = new Dictionary<int, float>();
    private GameObject[] enemyPops;
    private GameObject[] enemyGroundPops;

    private Canvas battleCanvas;
    private Slider hpSlider;
    private Slider mpSlider;
    private Text totalText;
    private Text killText;
    private Text lostText;
    private Text hitText;
    private Text messageText;
    private bool isBattleStart = false;
    private int totalScore = 0;
    private int killScore = 0;
    private int lostScore = 0;
    private int hitScore = 0;

    private PlayerController playerCtrl;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        stageNo = AppManager.Instance.stageNo;
        battleCanvas = GameObject.Find("BattleCanvas").GetComponent<Canvas>();
        battleCanvas.worldCamera = Camera.main;
        hpSlider = battleCanvas.transform.Find("BattleStatus/PlayerStatus/HP").GetComponent<Slider>();
        hpSlider.value = 1;
        mpSlider = battleCanvas.transform.Find("BattleStatus/PlayerStatus/MP").GetComponent<Slider>();
        mpSlider.value = 1;
        Transform scoresTran = battleCanvas.transform.Find("BattleStatus");
        totalText = scoresTran.Find("Total/Score").GetComponent<Text>();
        killText = scoresTran.Find("Scores/Kill/Score").GetComponent<Text>();
        lostText = scoresTran.Find("Scores/Lost/Score").GetComponent<Text>();
        hitText = scoresTran.Find("Scores/Hit/Score").GetComponent<Text>();
        SetScoreText();
        messageText = battleCanvas.transform.Find("Message").GetComponent<Text>();

        //敵情報（仮）
        popEnemy.Add(1, Resources.Load<GameObject>("Enemies/Enemy"));
        popInterval.Add(1, (stageNo == 1) ? 1.5f : 1.2f);
        popTime.Add(1, 0);

        popEnemy.Add(2, Resources.Load<GameObject>("Enemies/Enemy2"));
        popInterval.Add(2, (stageNo == 1) ? 0.75f: 0.5f);
        popTime.Add(2, 0);

        popEnemy.Add(3, Resources.Load<GameObject>("Enemies/Enemy3"));
        popInterval.Add(3, 0.5f);
        popTime.Add(3, 0);

        enemyPops = GameObject.FindGameObjectsWithTag(Common.CO.TAG_ENEMY_POP);
        enemyGroundPops = GameObject.FindGameObjectsWithTag(Common.CO.TAG_ENEMY_POP_GROUND);
    }

    private void Start()
    {

        StartCoroutine(BattleStart());
    }

    private void Update()
    {
        if (playerCtrl != null)
        {
            hpSlider.value = playerCtrl.GetHpRate();
            mpSlider.value = playerCtrl.GetMpRate();
        }
        else
        {
            hpSlider.value = 0;
        }

        if (!isBattleStart) return;

        battleTime += Time.deltaTime;
        foreach (int i in popEnemy.Keys)
        {
            EnemyPop(i);
        }
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
        yield return StartCoroutine(PlayerSummon());

        InputManager.Instance.SetActive(true);
        SetMessage("START", 3.0f);

        isBattleStart = true;
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

    public void EnemyPop(int index)
    {
        float diffTime = battleTime - popTime[index];
        if (diffTime >= popInterval[index])
        {
            Vector3 diffPos = Vector3.zero;
            GameObject[] pops = enemyGroundPops;
            if (!popEnemy[index].GetComponent<UnitController>().IsGravity())
            {
                diffPos = new Vector3(Common.FUNC.GetRandom(1), Common.FUNC.GetRandom(10), 0);
                pops =  enemyPops;
            }
            GameObject pop = Common.FUNC.RandomArray(pops);
            popTime[index] = battleTime;
            Instantiate(popEnemy[index], pop.transform.position + diffPos, pop.transform.rotation);
        }
    }

    public void AddKill()
    {
        killScore += 1;
        SetScoreText();
    }

    public void AddLost()
    {
        lostScore += 1;
        SetScoreText();
    }
    public void AddHit()
    {
        hitScore += 1;
        SetScoreText();
    }

    protected void SetScoreText()
    {
        //totalText.text = totalScore.ToString();
        killText.text = killScore.ToString();
        lostText.text = lostScore.ToString();
        hitText.text = hitScore.ToString();
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

    void OnApplicationPause(bool flg)
    {
        if (flg)
        {
            Pause();
        }
    }

    //### MENU ###

    public void Pause()
    {
        Time.timeScale = 0;
    }
    public void ResetPause()
    {
        Time.timeScale = 1;
    }

    public void Return()
    {
        ResetPause();
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_TITLE);
    }
}