using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEnemyController : UnitController
{
    protected GameObject playerObj;

    protected override void Awake()
    {
        maxHp = Mathf.RoundToInt(maxHp * BattleManager.Instance.GetPowRate());

        base.Awake();
    }

    protected override void Update()
    {
        if (BattleManager.Instance.IsBattleEnd()) return;
        base.Update();

        SearchPlayer();
    }

    protected virtual void SearchPlayer()
    {
        if (playerObj == null)
        {
            playerObj = GameObject.FindGameObjectWithTag(Common.CO.TAG_PLAYER);
        }
    }

    protected override void Dead()
    {
        BattleManager.Instance.AddKill();
        base.Dead();
    }

}