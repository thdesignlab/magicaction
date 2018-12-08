using UnityEngine;
using System.Collections;

public class LockOnBulletController : DamageObjectController
{
    [SerializeField]
    protected float firstSpeed;
    [SerializeField]
    protected float atackSpeed;
    [SerializeField]
    protected float stopTime;
    [SerializeField]
    protected float randomSpeed;
    [SerializeField]
    protected float randomTime;
    [SerializeField]
    protected GameObject lockOnObj;

    protected bool isFire = false;
    protected Transform targetTran;
    protected Transform lockOnSiteTran;
    protected LockOnFiringWeaponController lockOnWeaponCtrl;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FirstMove());
    }

    IEnumerator FirstMove()
    {
        SetSpeed(GetForward() * firstSpeed);
        stopTime += Common.FUNC.GetRandom(randomTime);
        for (; ; )
        {
            stopTime -= Time.deltaTime;
            liveTime = 0;
            if (stopTime <= 0) break;
            yield return null;
        }
        if (isFire) yield break;
        SetSpeed(Vector2.zero);
    }

    public void Fire(Vector2 target)
    {
        isFire = true;
        Common.FUNC.LookAt(myTran, target);
        SetSpeed(GetForward() * (atackSpeed + Common.FUNC.GetRandom(randomSpeed)));
    }
    public void Fire()
    {
        Vector2 v = (targetTran != null) ? (Vector2)targetTran.position : new Vector2(Common.FUNC.GetRandom(30), Common.FUNC.GetRandom(30));
        Fire(v);
    }

    public void SetTarget(Transform t)
    {
        if (t == null) return;
        targetTran = t;
        if (lockOnObj != null)
        {
            GameObject obj = Instantiate(lockOnObj, targetTran.position, Quaternion.identity);
            lockOnSiteTran = obj.transform;
            lockOnSiteTran.SetParent(targetTran, true);
        }
        lockOnWeaponCtrl = weapon.GetComponent<LockOnFiringWeaponController>();
    }

    public override void Break(bool isSpawn = true)
    {
        if (lockOnSiteTran != null)
        {
            Destroy(lockOnSiteTran.gameObject);
        }
        if (lockOnWeaponCtrl != null && targetTran != null)
        {
            lockOnWeaponCtrl.RemoveTarget(targetTran);
        }
        base.Break(isSpawn);
    }
}