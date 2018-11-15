using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleManager : SingletonMonoBehaviour<BattleManager>
{
    private float battleTime = 0;
    private Dictionary<int, GameObject> popEnemy = new Dictionary<int, GameObject>();
    private Dictionary<int, Transform> popTransform = new Dictionary<int, Transform>();
    private Dictionary<int, float> popInterval = new Dictionary<int, float>();
    private Dictionary<int, float> popTime = new Dictionary<int, float>();

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();
    }

    private void Start()
    {
        popEnemy.Add(1, Resources.Load<GameObject>("Units/Enemy"));
        popTransform.Add(1, GameObject.Find("Pops/Pop1").transform);
        popInterval.Add(1, 2.0f);
        popTime.Add(1, 0);

        popEnemy.Add(2, Resources.Load<GameObject>("Units/Enemy2"));
        popTransform.Add(2, GameObject.Find("Pops/Pop2").transform);
        popInterval.Add(2, 1.5f);
        popTime.Add(2, 0);

        popEnemy.Add(3, Resources.Load<GameObject>("Units/Enemy3"));
        popTransform.Add(3, GameObject.Find("Pops/Pop3").transform);
        popInterval.Add(3, 1.0f);
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
}