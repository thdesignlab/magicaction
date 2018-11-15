using UnityEngine;
using System.Collections;

public class DamageObjectController : PhysicsController
{
    [SerializeField]
    protected int damage;

    protected override void Update()
    {
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

        bool isScrape = true;
        switch (obj.tag)
        {
            case Common.CO.TAG_PLAYER:
                if (isPlayer) return;
                isScrape = false;
                break;

            case Common.CO.TAG_ENEMY:
                if (!isPlayer) return;
                isScrape = true;
                break;

            default:
                break;
        }

        UnitController unitCtrl = null;
        if (damage > 0 || isScrape) unitCtrl = obj.GetComponent<UnitController>();
        if (damage > 0) unitCtrl.Damage(damage);
        if (isScrape)
        {
            Scrape(unitCtrl.GetStrength());
        } else if (isBreakable)
        {
            Break();
        }
    }

    //ダメージオブジェクトに衝突
    protected override void HitDamageObject(GameObject obj)
    {
        base.HitDamageObject(obj);

        DamageObjectController dmgObjCtrl = obj.GetComponent<DamageObjectController>();
    }

    //ステージに衝突
    protected override void HitStage(GameObject obj)
    {
        base.HitStage(obj);

        switch (obj.tag)
        {
            case Common.CO.TAG_OBJECT:
                PhysicsController phyCtrl = obj.GetComponent<PhysicsController>();
                phyCtrl.Scrape(strength, this);
                break;

            default:
                if (tag == Common.CO.TAG_EFFECT) return;
                Break();
                break;
        }
    }
}