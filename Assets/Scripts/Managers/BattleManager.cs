using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    private float battleTime = 0;
    private Dictionary<int, GameObject> popEnemy = new Dictionary<int, GameObject>();
    private Dictionary<int, Transform> popTransform = new Dictionary<int, Transform>();
    private Dictionary<int, float> popInterval = new Dictionary<int, float>();
    private Dictionary<int, float> popTime = new Dictionary<int, float>();

    private Canvas battleCanvas;
    private Slider hpSlider;
    private Slider mpSlider;
    private Text scoreText;
    private Text messageText;
    private bool isBattleStart = false;

    private PlayerController playerCtrl;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        battleCanvas = GameObject.Find("BattleCanvas").GetComponent<Canvas>();
        hpSlider = battleCanvas.transform.Find("HP").GetComponent<Slider>();
        hpSlider.value = 1;
        mpSlider = battleCanvas.transform.Find("MP").GetComponent<Slider>();
        mpSlider.value = 1;
        scoreText = battleCanvas.transform.Find("Score").GetComponent<Text>();
        scoreText.text = "0";
        messageText = battleCanvas.transform.Find("Message").GetComponent<Text>();
    }

    private void Start()
    {
        BgmManager.Instance.PlayBgm();

        //敵情報（仮）
        popEnemy.Add(1, Resources.Load<GameObject>("Enemies/Enemy"));
        popTransform.Add(1, GameObject.Find("EnemyPops/Pop1").transform);
        popInterval.Add(1, 1.5f);
        popTime.Add(1, 0);

        popEnemy.Add(2, Resources.Load<GameObject>("Enemies/Enemy2"));
        popTransform.Add(2, GameObject.Find("EnemyPops/Pop2").transform);
        popInterval.Add(2, 0.75f);
        popTime.Add(2, 0);

        popEnemy.Add(3, Resources.Load<GameObject>("Enemies/Enemy3"));
        popTransform.Add(3, GameObject.Find("EnemyPops/Pop3").transform);
        popInterval.Add(3, 0.5f);
        popTime.Add(3, 0);

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
        EnemyPop(1);
        EnemyPop(2);
        EnemyPop(3);
    }

    IEnumerator BattleStart()
    {
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
        Transform pPopTran = GameObject.Find("PlayerPop").transform;

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
            popTime[index] = battleTime;
            GameObject obj = Instantiate(popEnemy[index], popTransform[index].position, popTransform[index].rotation);
        }
    }

    public void Return()
    {
        ScreenManager.Instance.SceneLoad(Common.CO.SCENE_TITLE);
    }

    public void AddScore()
    {
        scoreText.text = (int.Parse(scoreText.text) + 1).ToString();
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
}