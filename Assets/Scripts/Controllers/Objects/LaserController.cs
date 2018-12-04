using UnityEngine;
using System.Collections;

public class LaserController : DamageObjectController
{
    [SerializeField]
    protected Transform laserHead;
    [SerializeField]
    protected float maxLength;
    [SerializeField]
    protected float scaleTime;

    protected BoxCollider2D boxCollider;
    protected float defaultDamage;
    protected float defaultStrength;


    protected override void Awake()
    {
        base.Awake();

        boxCollider = myTran.GetComponent<BoxCollider2D>();
        defaultDamage = damage;
        defaultStrength = GetStrength();
        SetLaserHead(maxLength);
    }

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
        base.Update();
        if (player != null) liveTime = 0;
        LayerMask mask = Common.FUNC.GetLayerMask(Common.CO.LAYER_OBJECT);
        RaycastHit2D hit = Physics2D.Raycast(myTran.position, GetForward(), maxLength, mask);
        if (hit)
        {
            SetLaserHead(hit.point);
        }
        else
        {
            SetLaserHead(maxLength);
        }

    }

    protected void SetLaserHead(Vector2 pos)
    {
        float length = ((Vector2)myTran.position - pos).magnitude;
        SetLaserHead(length);
    }
    protected void SetLaserHead(float length)
    {
        if (length > maxLength) length = maxLength;
        laserHead.localPosition = new Vector2(length, 0);
        boxCollider.size = new Vector2(length, boxCollider.size.y);
        boxCollider.offset = new Vector2(length / 2, boxCollider.offset.y);
    }

    //ステージに衝突
    protected override void HitStage(GameObject obj)
    {
        switch (obj.tag)
        {
            case Common.CO.TAG_OBJECT:
            case Common.CO.TAG_EQUIP_OBJECT:
                PhysicsController phyCtrl = obj.GetComponent<PhysicsController>();
                if (phyCtrl.IsPlayer() != IsPlayer())
                {
                    float myStrength = GetStrength();
                    phyCtrl.Scrape(myStrength);
                }
                break;
        }
    }

    //### イベントハンドラ ###

    //衝突判定
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        return;
    }

    //接触判定
    protected void OnTriggerStay2D(Collider2D other)
    {
        GameObject targetObj = other.gameObject;
        string targetTag = targetObj.tag;
        damage = defaultDamage * deltaTime;
        strength = defaultStrength * deltaTime;

        //衝突対象判定
        if (Common.FUNC.IsUnitTag(targetTag))
        {
            HitUnit(targetObj);
        }
        else if (Common.FUNC.IsDamageObjectTag(targetTag))
        {
            Debug.Log(strength);
            HitDamageObject(targetObj);
        }
        else if (Common.FUNC.IsStageTag(targetTag))
        {
            HitStage(targetObj);
        }
    }
}