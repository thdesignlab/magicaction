using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : UnitController
{
    [SerializeField]
    private float speed;
    [SerializeField, HeaderAttribute("NormalAttack")]
    private GameObject bullet;
    [SerializeField]
    private float attackInterval;
    [SerializeField]
    private float rapidCount;
    [SerializeField]
    private float rapidInterval;
    [SerializeField]
    private float deviation;

    private float nextAttackTime;
    private bool isBulletGravity = false;
    private int bulletSpeed;

    protected GameObject playerObj;

    protected override void Awake()
    {
        maxHp = Mathf.RoundToInt(maxHp * BattleManager.Instance.GetPowRate());

        base.Awake();

        rapidCount = (rapidCount > 0) ? rapidCount: 1;
        nextAttackTime = attackInterval;
        if (bullet != null)
        {
            BulletController bulletCtrl = bullet.GetComponent<BulletController>();
            if (bulletCtrl != null)
            {
                isBulletGravity = bulletCtrl.IsGravity();
                bulletSpeed = bulletCtrl.GetSpeed();
            }
        }
    }

    protected override void Update()
    {
        if (BattleManager.Instance.IsBattleEnd()) return;
        base.Update();

        if (playerObj == null)
        {
            playerObj = GameObject.FindGameObjectWithTag(Common.CO.TAG_PLAYER);
        }
        nextAttackTime -= deltaTime;
        if (attackInterval > 0 && nextAttackTime <= 0)
        {
            Vector2 targetPos = (playerObj != null) ? playerObj.transform.position : myTran.position;
            StartCoroutine(Rapid(targetPos));
            nextAttackTime = attackInterval;
        }
    }

    protected override void InitSpeed()
    {
        base.InitSpeed();

        SetSpeed(GetForward() * speed);
    }

    protected virtual IEnumerator Rapid(Vector2 targetPos)
    {
        if (OnChant(1, true))
        {
            yield return new WaitForSeconds(3.0f);
        }

        
        if (isBulletGravity)
        {
            targetPos += Vector2.up * ((Vector2)myTran.position - targetPos).magnitude * 2;
        }
        for (int i = 0; i < rapidCount; i++)
        {
            foreach (Transform muzzle in muzzles)
            {
                Vector2 target = Common.FUNC.GetTargetWithDeviation(muzzle.position, targetPos, deviation);
                Spawn(bullet, target, muzzle);
                yield return null;
            }
            yield return new WaitForSeconds(rapidInterval);
        }

        OnChant(1, false);
    }
    private void Spawn(GameObject spawnObj, Vector2 target, Transform muzzleTran)
    {
        GameObject obj = Instantiate(spawnObj, muzzleTran.position + new Vector3(1, 0, 0), muzzleTran.rotation);
        Common.FUNC.LookAt(obj.transform, target);
    }

    protected override void Dead()
    {
        BattleManager.Instance.AddKill();
        base.Dead();
    }

    protected virtual void Assault(GameObject obj)
    {
        float d = (strength == 0) ? hp * hp : hp * strength;
        obj.GetComponent<UnitController>().Damage((int)d);
        base.Dead();
    }

    protected override void OutOfArea()
    {
        BattleManager.Instance.AddLost();
        base.OutOfArea();
    }

    //### イベントハンドラ ###

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.tag != Common.CO.TAG_PLAYER) return;

        Assault(collision.gameObject);
    }
}