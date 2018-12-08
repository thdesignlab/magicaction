using UnityEngine;
using System.Collections;

public class PhysicsController : ObjectController
{
    [SerializeField]
    private bool isGravity;
    [SerializeField]
    protected float strength;

    protected Rigidbody2D myBody;
    protected Vector2 myVelocity = Vector2.zero;
    protected Vector2 gVelocity = Vector2.zero;
    protected Vector2 myG;
    protected bool isGround = false;
    protected GameObject groundObj;
    protected bool isKnockBack = false;
    protected float floatPower = 0;
    protected float maxStrength;

    protected override void Awake()
    {
        base.Awake();
        myBody = GetComponent<Rigidbody2D>();
        myG = Physics2D.gravity;
        maxStrength = strength;
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (isGround && groundObj == null)
        {
            SetGround();
        }
    }
    protected virtual void FixedUpdate()
    {
        if (isGravity)
        {
            AddGravity(myG * Time.fixedDeltaTime);
        }
        Move(GetTotalVelocity() * Time.fixedDeltaTime);
    }
    protected virtual Vector2 GetTotalVelocity()
    {
        return myVelocity + gVelocity;
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
        if (isKnockBack) return;

        gVelocity += v;
        if (myG.y < gVelocity.y)
        {
            gVelocity = myG;
        }
    }

    //ノックバック
    protected Coroutine knockBackCoroutine;
    public void KnockBack(Vector3 v, float limit)
    {
        if (isKnockBack) return;
        if (isGround && v.normalized.y <= -0.5f) return;
        knockBackCoroutine = StartCoroutine(KnockBackProcess(v, limit));
    }

    protected void CancelKnockBack()
    {
        isKnockBack = false;
        if (knockBackCoroutine == null) return;
        StopCoroutine(knockBackCoroutine);
    }

    const float START_ANGLE = 90;
    const float TOTAL_ANGLE = 90;
    IEnumerator KnockBackProcess(Vector2 v, float limit)
    {
        if (v == Vector2.zero) yield break;

        isKnockBack = true;
        float leftTime = limit;
        for (; ; )
        {
            //時間
            float processTime = limit - leftTime;

            //速度係数
            float sinVal = Common.FUNC.GetSin(processTime, TOTAL_ANGLE / limit, START_ANGLE);

            //移動
            Move(v * sinVal * Time.deltaTime);

            //残り時間チェック
            leftTime -= Time.deltaTime;
            if (leftTime <= 0) break;
            yield return null;
        }
        isKnockBack = false;
    }

    //ユニットに衝突
    protected virtual bool HitUnit(GameObject obj)
    {
        return false;
    }

    //ダメージオブジェクトに衝突
    protected virtual bool HitDamageObject(GameObject obj)
    {
        return false;
    }

    //ステージに衝突
    protected virtual bool HitStage(GameObject obj)
    {
        return false;
    }

    //耐久値削減
    public void Scrape(int power = 0)
    {
        if (power < 0) return;
        
        strength -= (power == 0) ? strength : power;
        if (strength <= 0)
        {
            Break();
        }
    }
    public void Scrape(float power)
    {
        floatPower += power;
        if (floatPower < 1) return;

        int p = Mathf.FloorToInt(floatPower);
        Scrape(p);
        floatPower -= p;
    }

    //接地処理
    protected void SetGround(GameObject ground = null)
    {
        groundObj = ground;
        if (groundObj != null)
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

    //衝突時共通処理
    protected virtual void HitAction(Collider2D other)
    {

    }

    //### イベントハンドラ ###

    //衝突判定
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        GameObject targetObj = other.gameObject;
        string targetTag = targetObj.tag;
        bool isHit = false;

        //衝突対象判定
        if (Common.FUNC.IsUnitTag(targetTag))
        {
            isHit = HitUnit(targetObj);
        }
        else if (Common.FUNC.IsDamageObjectTag(targetTag))
        {
            isHit = HitDamageObject(targetObj);
        }
        else if (Common.FUNC.IsStageTag(targetTag))
        {
            isHit = HitStage(targetObj);
        }

        if (isHit) HitAction(other);
    }

    //### getter/setter ###

    public float GetStrength()
    {
        if (tag == Common.CO.TAG_EFFECT && strength == 0) return -1;
        return strength;
    }

    public bool IsGravity()
    {
        return isGravity;
    }
}