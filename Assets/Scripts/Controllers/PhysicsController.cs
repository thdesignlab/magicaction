using UnityEngine;
using System.Collections;

public class PhysicsController : ObjectController
{
    [SerializeField]
    private bool isGravity;
    [SerializeField]
    protected int strength;

    protected Rigidbody2D myBody;
    protected Vector2 myVelocity = Vector2.zero;
    protected Vector2 gVelocity = Vector2.zero;
    protected bool isBreakable;
    protected Vector2 myG;
    protected bool isGround = false;

    protected override void Awake()
    {
        base.Awake();
        myBody = GetComponent<Rigidbody2D>();
        isBreakable = (strength > 0);
        myG = Physics2D.gravity;
    }

    protected override void Update()
    {
        base.Update();
    }
    void FixedUpdate()
    {
        if (isGravity)
        {
            AddGravity(myG * Time.fixedDeltaTime);
        }
        Move((myVelocity + gVelocity) * Time.fixedDeltaTime);
    }

    //前方Vector取得
    protected Vector2 GetForward()
    {
        return myTran.right;
    }
    //後方Vector取得
    protected Vector2 GetBack()
    {
        return GetForward() * -1;
    }
    //上方Vector取得
    protected Vector2 GetUp()
    {
        return myTran.up;
    }
    //下方Vector取得
    protected Vector2 GetDown()
    {
        return GetUp() * -1;
    }

    //移動
    protected virtual void Move(Vector2 v)
    {
        if (v == Vector2.zero) return;
        myTran.position += Common.FUNC.ParseVector3(v);
    }

    //速度設定
    protected virtual void SetSpeed(Vector2 v)
    {
        myVelocity = v;
    }

    //加速
    protected virtual void AddSpeed(Vector2 v)
    {
        myVelocity += v;
    }

    //重力速度加算
    protected virtual void AddGravity(Vector2 v)
    {
        gVelocity += v;
        if (myG.y < gVelocity.y)
        {
            gVelocity = myG;
        }
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
        //Debug.Log("### HitUnit");
    }

    //ダメージオブジェクトに衝突
    protected virtual void HitDamageObject(GameObject obj)
    {
        //Debug.Log("### HitDamageObject");
    }

    //ステージに衝突
    protected virtual void HitStage(GameObject obj)
    {
        //Debug.Log("### HitStage");
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

    //接地処理
    protected void SetGround(bool flg)
    {
        if (flg)
        {
            gVelocity = Vector2.zero;
            myG = Vector2.zero;
            isGround = true;
        }
        else
        {
            myG = Physics2D.gravity;
            isGround = false;
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

    //### getter/setter ###

    public int GetStrength()
    {
        return strength;
    }

}