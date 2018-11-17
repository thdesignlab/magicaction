using UnityEngine;
using System.Collections;

public class UnitController : PhysicsController
{
    [SerializeField]
    protected int maxhp;

    protected int hp;
    protected float colliderRadius;
    protected float stackTime = 0;

    protected override void Awake()
    {
        base.Awake();

        hp = maxhp;
        SetHp();
        SetColliderRadius();
    }

    protected override void Start()
    {
        base.Start();

        InitSpeed();
    }

    public virtual void Damage(int damage)
    {
        SetHp(-damage);
        if (hp <= 0) Dead();
    }

    protected virtual void Dead()
    {
        Break();
    }

    protected virtual void SetHp(int diff = 0)
    {
        hp += diff;
        if (hp > maxhp) hp = maxhp;
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

    //### イベントハンドラ ###

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 p = contact.point - Common.FUNC.ParseVector2(myTran.position);
            if (p.y < 0)
            {
                //Unitの下方で衝突
                SetGround(true);
            }
            FlickFromStage(p);
        }
    }
    protected void OnCollisionStay2D(Collision2D collision)
    {
        if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 p = contact.point - Common.FUNC.ParseVector2(myTran.position);
            if ((p.x > 0 && myVelocity.x > 0) || (p.x < 0 && myVelocity.x < 0))
            {
                //Unitの前方で衝突
                if (p.normalized.y >= -0.8f)
                {
                    //乗り越えられないオブジェクトと衝突
                    StartCoroutine(Stack(1.5f));
                }
            }
            FlickFromStage(p);
        }
    }
    protected void OnCollisionExit2D(Collision2D collision)
    {
        if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        SetGround(false);
    }
}