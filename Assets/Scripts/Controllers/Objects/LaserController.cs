using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LaserController : DamageObjectController
{
    [SerializeField]
    protected GameObject collisionEffect;
    [SerializeField]
    protected Transform laserHead;
    [SerializeField]
    protected float maxLength;
    [SerializeField]
    protected float scaleTime;

    protected BoxCollider2D boxCollider;
    protected float defaultDamage;
    protected float defaultStrength;
    protected List<Collider2D> stayList = new List<Collider2D>();

    protected string[] raycastHitLayers = new string[] {
        Common.CO.LAYER_ENEMY_BOSS,
        Common.CO.LAYER_OBJECT,
    };

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

        damage = defaultDamage * deltaTime;
        strength = defaultStrength * deltaTime;

        if (player != null) liveTime = 0;
        bool isHitEffect = false;
        LayerMask mask = Common.FUNC.GetLayerMask(raycastHitLayers);
        RaycastHit2D[] hits = Physics2D.RaycastAll(myTran.position, GetForward(), maxLength, mask);
        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                ObjectController objCtrl = hit.collider.GetComponent<ObjectController>();
                if (objCtrl == null || !objCtrl.IsPlayer())
                {
                    SetLaserHead(hit.point);
                    isHitEffect = true;
                    break;
                }
            }
        }
        if (!isHitEffect)
        {
            SetLaserHead(maxLength);
        }
        SwitchHitEffect(isHitEffect);
        StayAction();
    }

    //レーザー先端設定
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

    protected void SwitchHitEffect(bool flg)
    {
        if (collisionEffect != null) collisionEffect.SetActive(flg);
    }

    //ステージに衝突
    protected override bool HitStage(GameObject obj)
    {
        bool isHit = false;
        switch (obj.tag)
        {
            case Common.CO.TAG_OBJECT:
            case Common.CO.TAG_EQUIP_OBJECT:
                PhysicsController phyCtrl = obj.GetComponent<PhysicsController>();
                if (phyCtrl.IsPlayer() != IsPlayer())
                {
                    float myStrength = GetStrength();
                    phyCtrl.Scrape(myStrength);
                    isHit = true;
                }
                break;
        }

        return isHit;
    }

    //衝突Colliderリスト
    protected void EnterColliderList(Collider2D other)
    {
        if (stayList.Contains(other)) return;
        stayList.Add(other);
    }
    protected void ExitColliderList(Collider2D other)
    {
        stayList.Remove(other);
    }

    //接触中処理
    protected void StayAction()
    {
        stayList.RemoveAll(o => o == null);
        List<Collider2D> tempList = new List<Collider2D>(stayList);
        foreach (Collider2D other in tempList)
        {
            if (other == null) continue;
            GameObject targetObj = other.gameObject;
            string targetTag = targetObj.tag;

            //衝突対象判定
            if (Common.FUNC.IsUnitTag(targetTag))
            {
                HitUnit(targetObj);
            }
            else if (Common.FUNC.IsDamageObjectTag(targetTag))
            {
                HitDamageObject(targetObj);
            }
            else if (Common.FUNC.IsStageTag(targetTag))
            {
                HitStage(targetObj);
            }
        }
    }

    //### イベントハンドラ ###

    //衝突判定
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        EnterColliderList(other);
    }

    //接触判定
    protected void OnTriggerStay2D(Collider2D other)
    {
        EnterColliderList(other);
    }

    //接触判定
    protected void OnTriggerExit2D(Collider2D other)
    {
        ExitColliderList(other);
    }
}