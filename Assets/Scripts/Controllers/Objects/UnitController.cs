using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitController : PhysicsController
{
    [SerializeField]
    protected int maxHp;

    protected int hp = 0;
    protected float colliderRadius;
    protected float stackTime = 0;

    //詠唱エフェクト
    protected Dictionary<string, ParticleSystem> chantDic = new Dictionary<string, ParticleSystem>();

    //バリアエフェクト
    protected GameObject barrier;
    protected float barrierLeftTime = 0;

    //バリア時間
    const float BARRIER_LIMIT = 1.0f;
    //乗り越え制限
    const float ENABLED_OVER_RATE = 0.2f;

    protected override void Awake()
    {
        base.Awake();

        SetHp(maxHp);
        SetColliderRadius();
        SetChant();
        SetBarrier();
    }

    protected override void Start()
    {
        base.Start();

        InitSpeed();
    }

    //被弾
    public virtual void Damage(int damage)
    {
        if (hp <= 0) return;
        OnBarrier();
        SetHp(-damage);
        if (hp <= 0) Dead();
    }

    //死亡
    protected virtual void Dead()
    {
        Break();
    }

    //HP設定
    protected virtual void SetHp(int diff = 0)
    {
        hp += diff;
        if (hp > maxHp) hp = maxHp;
    }

    //初期速度設定
    protected virtual void InitSpeed()
    {

    }

    //移動停止
    protected virtual IEnumerator Stack(float time)
    {
        if (stackTime > 0)
        {
            stackTime += time;
            yield break;
        }

        stackTime = time;
        SetSpeed(Vector2.zero);

        for (; ; )
        {
            stackTime -= deltaTime;
            if (stackTime <= 0)
            {
                stackTime = 0;
                break;
            }
            yield return null;
        }

        InitSpeed();
    }

    //Stageに重なった時にはじく
    protected void FlickFromStage(Vector2 p)
    {
        float diff = colliderRadius - p.magnitude;
        if (diff > 0.1f)
        {
            if (isKnockBack) CancelKnockBack();
            Vector2 flickVector = -p.normalized;
            if (isGround && flickVector.y < 0) flickVector.y = 0;
            Move(flickVector * diff * 5.0f);
        }
    }

    //コライダーの半径設定
    protected void SetColliderRadius()
    {
        CircleCollider2D myCircleCollider = GetComponent<CircleCollider2D>();
        if (myCircleCollider != null)
        {
            colliderRadius = myCircleCollider.radius;
            return;
        }
        colliderRadius = 0;
    }

    //詠唱エフェクト設定
    protected void SetChant()
    {
        Transform parts = myTran.Find(Common.UNIT.PARTS_CHANT);
        if (parts == null) return;
        foreach (Transform child in parts) {
            chantDic.Add(child.name, child.gameObject.GetComponent<ParticleSystem>());
        }
    }
    protected void OnChant(int level, bool flg)
    {
        string targetKey = Common.CO.LEVEL_PREFIX + level.ToString();
        float t = 0;
        ParticleSystem p = null;
        foreach (string key in chantDic.Keys)
        {
            if (!chantDic.ContainsKey(key)) continue;
            bool isActive = (targetKey == key && flg);
            if (chantDic[key].gameObject.activeInHierarchy) t = chantDic[key].time;
            chantDic[key].gameObject.SetActive(isActive);
            if (chantDic[key].gameObject.activeInHierarchy) p = chantDic[key];
        }
        if (p != null)
        {
            p.Simulate(t);
            p.Play();
        }
    }

    //バリアエフェクト
    protected void SetBarrier()
    {
        Transform barrierTran = myTran.Find("Barrier");
        if (barrierTran == null) return;
        barrier = barrierTran.gameObject;
    }
    protected void OnBarrier()
    {
        if (barrier == null) return;
        float preLeftTime = barrierLeftTime;
        barrierLeftTime = BARRIER_LIMIT;
        if (preLeftTime > 0) return;
        StartCoroutine(OnBarrierProcess());
    }
    IEnumerator OnBarrierProcess()
    {
        barrier.SetActive(true);
        for (; ; )
        {
            barrierLeftTime -= Time.deltaTime;
            if (barrierLeftTime <= 0) break;
            yield return null;
        }
        barrier.SetActive(false);
    }

    //### イベントハンドラ ###

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 p = contact.point - Common.FUNC.ParseVector2(myTran.position);
            if (p.y < 0)
            {
                //Unitの下方で衝突
                SetGround(collision.gameObject);
            }
            FlickFromStage(p);
        }
    }
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 p = contact.point - Common.FUNC.ParseVector2(myTran.position);
            if ((p.x > 0 && myVelocity.x > 0) || (p.x < 0 && myVelocity.x < 0))
            {
                //Unitの前方で衝突
                float height = (1 + p.normalized.y) * colliderRadius;
                if (height > colliderRadius * ENABLED_OVER_RATE)
                {
                    //乗り越えられないオブジェクトと衝突
                    StartCoroutine(Stack(1.5f));
                }
            }
            FlickFromStage(p);
        }
    }
    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        SetGround();
    }

    //### getter/setter ###

    public float GetColliderRadius()
    {
        return colliderRadius;
    }
}