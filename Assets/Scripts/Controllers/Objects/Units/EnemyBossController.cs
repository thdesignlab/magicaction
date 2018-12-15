using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;

public class EnemyBossController : BaseEnemyController
{
    [SerializeField]
    protected int bossId;
    [SerializeField]
    protected List<GameObject> weaponList = new List<GameObject>();
    [SerializeField]
    protected GameObject shieldWeapon;
    [SerializeField]
    protected float shieldInterval;

    protected bool isReady = false;
    protected float nextShieldTime = 0;
    protected EnemyWeaponController shieldWeaponCtrl;
    protected List<EnemyWeaponController> weaponCtrlList;
    protected InputStatus inputStatus = new InputStatus();
    protected List<GameObject> shieldObjList;
    protected CinemachineSmoothPath bossPath;
    protected CinemachineDollyCart bossCart;
    protected Transform bossCartTran;
    protected float bossCartSpeed;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        bossPath = BattleManager.Instance.GetBossPath();
        if (bossPath != null)
        {
            bossCart = BattleManager.Instance.GetBossCart();
            if (bossCart != null)
            {
                bossCartTran = bossCart.transform;
                bossCartSpeed = bossCart.m_Speed;
            }
        }
        StartCoroutine(Summon());
    }

    protected override void Update()
    {
        if (!isReady) return;

        base.Update();

        //シールド
        if (shieldWeaponCtrl != null)
        {
            shieldObjList = shieldWeaponCtrl.GetExistObject();
            if (shieldObjList.Count == 0)
            {
                //シールド展開
                nextShieldTime -= deltaTime;
                if (nextShieldTime <= 0)
                {
                    OnShield();
                    nextShieldTime = shieldInterval;
                }
            }
        }

        //移動
        if (bossCart != null) SetMoveVelocity(bossCartTran.position, bossCartSpeed);
    }

    protected override void SetMoveVelocity(Vector2 targetPos, float moveSpeed)
    {
        moveVelocity = Vector2.zero;
        bossCart.m_Speed = 0;
        if (!isKnockBack)
        {
            Vector2 targetVector = targetPos - Common.FUNC.ParseVector2(myTran.position);
            float distance = targetVector.magnitude;
            if (distance > 0.5f)
            {
                moveVelocity = targetVector.normalized * moveSpeed;
            }
            if (distance <= 1.0f)
            {
                bossCart.m_Speed = bossCartSpeed;
            }
        }
    }

    IEnumerator Summon()
    {
        SwitchBossChant(true);

        yield return new WaitForSeconds(1.5f);

        SwitchBossChant(false);
        isReady = true;
        if (bossCart != null) bossCart.m_Position = 0;
        StartCoroutine(FireSchedule());
    }

    IEnumerator FireSchedule()
    {
        Dictionary<int, float> fireSchedule = new Dictionary<int, float>()
        {
            { 0, 10 },
        };
        int fireIndex = 0;
        float fireTime = fireSchedule[fireIndex] / 2;
        float interval = 0.1f;
        for (; ; )
        {
            fireTime -= interval;
            if (fireTime <= 0)
            {
                Fire(weaponCtrlList, fireIndex);
                fireIndex = ++fireIndex % fireSchedule.Count;
                fireTime = fireSchedule[fireIndex];
            }
            yield return new WaitForSeconds(interval);
        }
    }

    protected override void SearchPlayer()
    {
        base.SearchPlayer();
        if (playerObj == null) return;

        inputStatus.point = playerObj.transform.position;
    }

    protected void Assault(GameObject obj)
    {
        obj.GetComponent<UnitController>().Damage(strength);
    }

    protected override void OutOfArea()
    {
        return;
    }

    //武器設定
    protected override void SetWeapon()
    {
        base.SetWeapon();
        weaponCtrlList = EquipEnemyWeapon(weaponList);
        shieldWeaponCtrl = EquipEnemyWeapon(shieldWeapon);
    }
    protected List<EnemyWeaponController>  EquipEnemyWeapon(List<GameObject> objList)
    {
        List<EnemyWeaponController> list = new List<EnemyWeaponController>();
        int l = 1;
        foreach (GameObject obj in objList)
        {
            list.Add(EquipEnemyWeapon(obj, l++));
        }
        return list;
    }
    protected EnemyWeaponController EquipEnemyWeapon(GameObject obj, int level = 1)
    {
        WeaponController wc = EquipWeapon(obj);
        EnemyWeaponController ewc = (EnemyWeaponController)wc;
        if (ewc != null) ewc.SetEnemy(this, level);
        return ewc;
    }
    protected EnemyWeaponController SelectWeapon(List<EnemyWeaponController> weaponCtrlList, int level = 0)
    {
        if (weaponCtrlList.Count == 0) return null;
        level = (weaponCtrlList.Count <= level) ? weaponCtrlList.Count - 1 : level;
        return weaponCtrlList[level];
    }

    //攻撃
    protected void Fire(List<EnemyWeaponController> weaponCtrlList, int level = 0)
    {
        EnemyWeaponController weaponCtrl = SelectWeapon(weaponCtrlList, level);
        if (weaponCtrl == null) return;
        Fire(weaponCtrl);
    }
    protected void Fire(EnemyWeaponController weaponCtrl)
    {
        if (weaponCtrl == null) return;
        weaponCtrl.Fire(inputStatus);
    }

    //シールド展開
    protected void OnShield()
    {
        shieldWeaponCtrl.Fire(inputStatus);
    }

    public void SwitchBossChant(bool flg, int level = 1)
    {
        base.OnChant(level, flg);
    }

    //### イベントハンドラ ###

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;
        //BreakableObjectController breakableObjCtrl = collision.gameObject.GetComponent<BreakableObjectController>();
        //if (breakableObjCtrl == null) return;
        base.OnCollisionEnter2D(collision);
    }
    protected override void OnCollisionStay2D(Collision2D collision)
    {
        //if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    Vector2 p = contact.point - Common.FUNC.ParseVector2(myTran.position);
        //    FlickFromStage(p);
        //}
        base.OnCollisionStay2D(collision);
    }
}
