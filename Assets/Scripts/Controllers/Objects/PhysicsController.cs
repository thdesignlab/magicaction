﻿using UnityEngine;
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
    protected GameObject groundObj;
    protected bool isKnockBack = false;


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