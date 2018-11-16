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
    private Text scoreText;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();

        battleCanvas = GameObject.Find("BattleCanvas").GetComponent<Canvas>();
        scoreText = battleCanvas.transform.Find("Score").GetComponent<Text>();
        scoreText.text = "0";
    }

    private void Start()
    {
        BgmManager.Instance.PlayBgm();

        popEnemy.Add(1, Resources.Load<GameObject>("Units/Enemy"));
        popTransform.Add(1, GameObject.Find("Pops/Pop1").transform);
        popInterval.Add(1, 3f);
        popTime.Add(1, 0);

        popEnemy.Add(2, Resources.Load<GameObject>("Units/Enemy2"));
        popTransform.Add(2, GameObject.Find("Pops/Pop2").transform);
        popInterval.Add(2, 0.9f);
        popTime.Add(2, 0);

        popEnemy.Add(3, Resources.Load<GameObject>("Units/Enemy3"));
        popTransform.Add(3, GameObject.Find("Pops/Pop3").transform);
        popInterval.Add(3, 0.7f);
        popTime.Add(3, 0);
    }

    private void Update()
    {
        battleTime += Time.deltaTime;
        EnemyPop(1);
        EnemyPop(2);
        EnemyPop(3);
    }

    private void EnemyPop(int index)
    {
        float diffTime = battleTime - popTime[index];
        if (diffTime >= popInterval[index])
        {
            popTime[index] = battleTime;
            GameObject obj = Instantiate<GameObject>(popEnemy[index], popTransform[index].position, popTransform[index].rotation);
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
}