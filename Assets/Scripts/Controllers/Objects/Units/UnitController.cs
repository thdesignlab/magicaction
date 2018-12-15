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
    protected float floatDamage = 0;
    protected Transform weaponsTran;
    protected Vector2 moveVelocity = Vector2.zero;

    //詠唱エフェクト
    protected Dictionary<string, GameObject> chantObjDic = new Dictionary<string, GameObject>();
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
        SetWeapon();
    }

    protected override void Start()
    {
        base.Start();

        InitSpeed();
    }

    protected virtual void SetMoveVelocity(Vector2 targetPos, float moveSpeed)
    {
        moveVelocity = Vector2.zero;
        if (!isKnockBack)
        {
            Vector2 targetVector = targetPos - Common.FUNC.ParseVector2(myTran.position);
            float distance = targetVector.magnitude;
            if (distance > 0.5f)
            {
                moveVelocity = targetVector.normalized * moveSpeed;
            }
        }
    }

    protected override Vector2 GetTotalVelocity()
    {
        return base.GetTotalVelocity() + moveVelocity;
    }

    //被弾
    public virtual bool Damage(int damage)
    {
        if (!myRenderer.isVisible || hp <= 0) return false;
        OnBarrier();
        SetHp(-damage);
        if (hp <= 0)
        {
            Dead();
            return true;
        }
        return false;
    }
    public virtual bool Damage(float damage)
    {
        floatDamage += damage;
        if (floatDamage < 1) return false;

        int d = Mathf.FloorToInt(floatDamage);
        bool isKill = Damage(d);
        floatDamage -= d;
        return isKill;
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

        strength = maxStrength;
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
        Transform parts = myTran.Find(Common.UNIT.PARTS_CHANTS);
        if (parts == null) return;
        foreach (Transform child in parts) {
            chantObjDic.Add(child.name, child.gameObject);
            //chantDic.Add(child.name, child.gameObject.GetComponentInChildren<ParticleSystem>());
        }
    }
    protected bool OnChant(int level, bool flg)
    {
        if (chantObjDic.Count == 0) return false;

        string targetKey = Common.CO.LEVEL_PREFIX + level.ToString();
        float t = 0;
        ParticleSystem p = null;
        foreach (string key in chantObjDic.Keys)
        {
            if (!chantObjDic.ContainsKey(key)) continue;
            bool isActive = (targetKey == key && flg);
            //if (chantDic.ContainsKey(key) && chantDic[key].gameObject.activeInHierarchy) t = chantDic[key].time;
            chantObjDic[key].gameObject.SetActive(isActive);
            //if (chantDic.ContainsKey(key) && chantDic[key].gameObject.activeInHierarchy) p = chantDic[key];
        }
        //if (p != null)
        //{
        //    p.Simulate(t);
        //    p.Play();
        //}
        return true;
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

    //武器設定
    protected virtual void SetWeapon()
    {
        weaponsTran = myTran.Find(Common.PLAYER.PARTS_WEAPONS);
    }
    public virtual WeaponController EquipWeapon(GameObject weapon)
    {
        if (weapon == null) return null;
        GameObject weaponObj = Instantiate(weapon, weaponsTran.position, Quaternion.identity);
        weaponObj.transform.SetParent(weaponsTran, true);
        WeaponController weaponCtrl = weaponObj.GetComponentInChildren<WeaponController>();
        return weaponCtrl;
    }
    protected virtual List<WeaponController> EquipWeapon(List<GameObject> weaponList)
    {
        List<WeaponController> wepaonCtrlList = new List<WeaponController>();
        foreach (GameObject weapon in weaponList)
        {
            wepaonCtrlList.Add(EquipWeapon(weapon));
        }
        return wepaonCtrlList;
    }

    //武器リストから対象レベルの武器を取得
    protected WeaponController SelectWeapon(List<WeaponController> weaponCtrlList, int level = 0)
    {
        if (weaponCtrlList.Count == 0) return null;
        level = (weaponCtrlList.Count <= level) ? weaponCtrlList.Count - 1 : level;
        return  weaponCtrlList[level];
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
                PhysicsController phyCtrl = collision.gameObject.GetComponent<PhysicsController>();
                if (phyCtrl != null && strength > 0 && phyCtrl.IsPlayer() != IsPlayer())
                {
                    float stageStrength = phyCtrl.GetStrength();
                    phyCtrl.Scrape(strength);
                    strength -= stageStrength;
                }
                if (strength <= 0)
                {
                    strength = 0;
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

    public int GetMaxHp()
    {
        return maxHp;
    }

    public int GetDamage()
    {
        return maxHp - hp;
    }
}