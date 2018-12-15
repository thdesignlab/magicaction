using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : BaseEnemyController
{
    [SerializeField]
    private float moveSpeed;
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

    protected override void Awake()
    {
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

        SetSpeed(GetForward() * moveSpeed);
    }

    protected virtual IEnumerator Rapid(Vector2 targetPos)
    {
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
    }
    private void Spawn(GameObject spawnObj, Vector2 target, Transform muzzleTran)
    {
        GameObject obj = Instantiate(spawnObj, muzzleTran.position + new Vector3(1, 0, 0), muzzleTran.rotation);
        Common.FUNC.LookAt(obj.transform, target);
    }

    protected void Assault(GameObject obj)
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