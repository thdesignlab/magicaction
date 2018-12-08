using UnityEngine;
using System.Collections;

public class DamageObjectController : PhysicsController
{
    [SerializeField]
    protected float damage;
    [SerializeField]
    protected GameObject hitEffect;

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
    protected override bool HitUnit(GameObject obj)
    {
        base.HitUnit(obj);

        if (obj.tag == Common.CO.TAG_PLAYER && IsPlayer()) return false;
        if (obj.tag != Common.CO.TAG_PLAYER && !IsPlayer()) return false;

        UnitController unitCtrl = obj.GetComponent<UnitController>();
        float enemyStrength = unitCtrl.GetStrength();
        if (damage > 0)
        {
            bool isKill = unitCtrl.Damage(damage);
            if (IsPlayer() && weapon != null)
            {
                weapon.AddDamageWR(index, damage, isKill);
            }
        }
        if (tag != Common.CO.TAG_EFFECT)
        {
            Scrape((int)enemyStrength);
        }

        return true;
    }

    //ダメージオブジェクトに衝突
    protected override bool HitDamageObject(GameObject obj)
    {
        base.HitDamageObject(obj);

        //弾同士の衝突はプレイヤー側が行う
        if (!IsPlayer()) return false;

        DamageObjectController dmgObjCtrl = obj.GetComponent<DamageObjectController>();

        //自分の弾同士はスルー
        if (dmgObjCtrl.IsPlayer()) return false;

        float myStrength = GetStrength();
        float enemyStrength = dmgObjCtrl.GetStrength();
        if (obj.tag != Common.CO.TAG_EFFECT)
        {
            //相手の弾の耐久値減少
            dmgObjCtrl.Scrape(myStrength);
        }
        if (tag != Common.CO.TAG_EFFECT)
        {
            //相手の弾による耐久値減少
            Scrape(enemyStrength);
        }

        return true;
    }

    //ステージに衝突
    protected override bool HitStage(GameObject obj)
    {
        base.HitStage(obj);

        //エフェクトの場合スルー
        if (tag == Common.CO.TAG_EFFECT) return false;

        switch (obj.tag)
        {
            case Common.CO.TAG_OBJECT:
            case Common.CO.TAG_EQUIP_OBJECT:
                PhysicsController phyCtrl = obj.GetComponent<PhysicsController>();
                if (phyCtrl.IsPlayer() != IsPlayer())
                {
                    float myStrength = GetStrength();
                    float enemyStrength = phyCtrl.GetStrength();
                    phyCtrl.Scrape(myStrength);
                    Scrape(enemyStrength);
                }
                break;

            default:
                Break();
                break;
        }

        return true;
    }

    //衝突時共通処理
    protected override void HitAction(Collider2D other)
    {
        if (hitEffect != null)
        {
            Vector2 point = other.bounds.ClosestPoint(myTran.position);
            Instantiate(hitEffect, point, Quaternion.identity);
        }
    }

}