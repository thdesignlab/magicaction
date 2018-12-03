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

    protected override void Awake()
    {
        base.Awake();

        boxCollider = myTran.GetComponent<BoxCollider2D>();
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
    }

    protected void SetLaserHead(Vector2 pos)
    {
        float length = ((Vector2)myTran.position - pos).magnitude;
        SetLaserHead(length);
    }
    protected void SetLaserHead(float length)
    {
        if (length > maxLength) length = maxLength;
        laserHead.position = new Vector2(length, 0);
        boxCollider.size = new Vector2(length, boxCollider.size.y);
        boxCollider.offset = new Vector2(length / 2, boxCollider.offset.y);
    }

    //### イベントハンドラ ###

    //衝突判定
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        GameObject targetObj = other.gameObject;
        string targetTag = targetObj.tag;

        Vector2 hitPos = other.bounds.ClosestPoint(myTran.position);
        //Debug.Log(targetObj.name+" >> "+ hitPos);

        //衝突対象判定
        SetLaserHead(hitPos);
        //if (Common.FUNC.IsUnitTag(targetTag))
        //{
        //    HitUnit(targetObj);
        //}
        //else if (Common.FUNC.IsDamageObjectTag(targetTag))
        //{
        //    HitDamageObject(targetObj);
        //}
        //else if (Common.FUNC.IsStageTag(targetTag))
        //{
        //    HitStage(targetObj);
        //}
    }

    //接触判定
    protected void OnTriggerStay2D(Collider2D other)
    {
        GameObject targetObj = other.gameObject;
        string targetTag = targetObj.tag;

        ////衝突対象判定
        //if (Common.FUNC.IsUnitTag(targetTag))
        //{
        //    HitUnit(targetObj);
        //}
        //else if (Common.FUNC.IsDamageObjectTag(targetTag))
        //{
        //    HitDamageObject(targetObj);
        //}
        //else if (Common.FUNC.IsStageTag(targetTag))
        //{
        //    HitStage(targetObj);
        //}

    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        SetLaserHead(maxLength);
    }
}