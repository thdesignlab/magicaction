using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    protected List<EnemyWeaponController> weaponListCtrl;
    protected InputStatus inputStatus = new InputStatus();
    protected List<GameObject> shieldObjList;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(Summon());
    }

    protected override void Update()
    {
        if (!isReady) return;

        base.Update();

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
    }

    IEnumerator Summon()
    {
        OnChant(1, true);

        yield return new WaitForSeconds(1.5f);

        OnChant(1, false);
        isReady = true;
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
        weaponListCtrl = EquipEnemyWeapon(weaponList);
        shieldWeaponCtrl = EquipEnemyWeapon(shieldWeapon);
    }
    protected List<EnemyWeaponController>  EquipEnemyWeapon(List<GameObject> objList)
    {
        List<EnemyWeaponController> list = new List<EnemyWeaponController>();
        foreach (GameObject obj in objList)
        {
            list.Add(EquipEnemyWeapon(obj));
        }
        return list;
    }
    protected EnemyWeaponController EquipEnemyWeapon(GameObject obj)
    {
        WeaponController wc = EquipWeapon(obj);
        return (EnemyWeaponController)wc;
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
