using UnityEngine;
using System.Collections;

public class DamageObjectController : PhysicsController
{
    [SerializeField]
    protected int damage;

    protected override void Start()
    {
        base.Start();
        if (!IsPlayer())
        {
            damage = Mathf.RoundToInt(damage * BattleManager.Instance.GetPowRate());
        }
    }

    protected override void Update()
    {
        if (BattleManager.Instance.IsBattleEnd())
        {
            Break();
            return;
        }
        base.Update();
        if (liveTime > 10.0f)
        {
            Break();
            return;
        }
    }

    //ユニットに衝突
    protected override void HitUnit(GameObject obj)
    {
        base.HitUnit(obj);

        if (obj.tag == Common.CO.TAG_PLAYER && IsPlayer()) return;
        if (obj.tag != Common.CO.TAG_PLAYER && !IsPlayer()) return;

        UnitController unitCtrl = obj.GetComponent<UnitController>();
        int enemyStrength = unitCtrl.GetStrength();
        if (damage > 0)
        {
            unitCtrl.Damage(damage);
        }
        if (tag != Common.CO.TAG_EFFECT)
        {
            Scrape(enemyStrength);
        }
    }

    //ダメージオブジェクトに衝突
    protected override void HitDamageObject(GameObject obj)
    {
        base.HitDamageObject(obj);

        //弾同士の衝突はプレイヤー側が行う
        if (!IsPlayer()) return;

        DamageObjectController dmgObjCtrl = obj.GetComponent<DamageObjectController>();

        //自分の弾同士はスルー
        if (dmgObjCtrl.IsPlayer()) return;

        int myStrength = GetStrength();
        int enemyStrength = dmgObjCtrl.GetStrength();

        if (tag != Common.CO.TAG_EFFECT)
        {
            //相手の弾による耐久値減少
            Scrape(enemyStrength);
        }
        if (obj.tag != Common.CO.TAG_EFFECT)
        {
            //相手の弾の耐久値減少
            dmgObjCtrl.Scrape(myStrength);
        }
    }

    //ステージに衝突
    protected override void HitStage(GameObject obj)
    {
        base.HitStage(obj);

        //エフェクトの場合スルー
        if (tag == Common.CO.TAG_EFFECT) return;

        switch (obj.tag)
        {
            case Common.CO.TAG_OBJECT:
            case Common.CO.TAG_EQUIP_OBJECT:
                PhysicsController phyCtrl = obj.GetComponent<PhysicsController>();
                if (phyCtrl.IsPlayer() != IsPlayer())
                {
                    int myStrength = GetStrength();
                    int enemyStrength = phyCtrl.GetStrength();
                    Scrape(enemyStrength);
                    phyCtrl.Scrape(myStrength);
                }
                break;

            default:
                Break();
                break;
        }
    }
}