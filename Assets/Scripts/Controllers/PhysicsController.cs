using UnityEngine;
using System.Collections;

public class PhysicsController : ObjectController
{
    [SerializeField]
    private bool isGravity;
    [SerializeField]
    protected int strength;

    protected Rigidbody2D myBody;
    protected bool isBreakable;

    protected override void Awake()
    {
        base.Awake();

        myBody = GetComponent<Rigidbody2D>();
        if (myBody != null)
        {
            myBody.gravityScale = isGravity ? 1 : 0;
        }
        isBreakable = (strength > 0);
    }

    protected override void Update()
    {
        base.Update();


    }

    //前方Vector取得
    protected Vector3 GetForward()
    {
        return myTran.right;
    }

    //移動
    protected virtual void Move(Vector3 vector, float speed)
    {
        myTran.position += vector * speed * deltaTime;
    }

    //前方へ移動
    protected virtual void MoveForward(float speed)
    {
        Move(GetForward(), speed);
    }

    //2DLookAt
    protected virtual void LookAt(Transform tran, Vector3 target)
    {
        Vector3 diff = (target - tran.position).normalized;
        tran.rotation = Quaternion.FromToRotation(Vector3.right, diff);
    }

    //ユニットに衝突
    protected virtual void HitUnit(GameObject obj)
    {
        //DebugManager.Instance.AdminLog("### HitUnit");
        //DebugManager.Instance.AdminLog(this.name, obj.name);
    }

    //ダメージオブジェクトに衝突
    protected virtual void HitDamageObject(GameObject obj)
    {
        //DebugManager.Instance.AdminLog("### HitDamageObject");
        //DebugManager.Instance.AdminLog(this.name, obj.name);
    }

    //ステージに衝突
    protected virtual void HitStage(GameObject obj)
    {
        //DebugManager.Instance.AdminLog("### HitStage");
        //DebugManager.Instance.AdminLog(this.name, obj.name);
    }

    //耐久値削減
    public void Scrape(int power = 1, PhysicsController phyCtrl = null)
    {
        if (!isBreakable || power <= 0) return;

        //相手の耐久値
        if (phyCtrl != null)
        {
            phyCtrl.Scrape(strength);
        }

        //自分の耐久値
        strength -= power;
        if (strength <= 0)
        {
            Break();
        }
    }

    //### イベントハンドラ ###

    //衝突判定
    protected void OnTriggerEnter2D(Collider2D other)
    {
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
    //protected void OnCollisionEnter2D(Collision2D collision)
    //{
    //}

    //### getter/setter ###

    public int GetStrength()
    {
        return strength;
    }

}