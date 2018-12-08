using UnityEngine;
using System.Collections;

public class TrackingBulletController : BulletController
{
    [SerializeField]
    protected float turnAngle;
    [SerializeField]
    protected GameObject targetSite;

    protected Transform target;
    protected Transform targetSiteTran;

    protected override void Update()
    {
        base.Update();

        if (target != null)
        {
            Common.FUNC.LookAt(myTran, target.position, turnAngle * Time.deltaTime);
            SetSpeed(GetForward() * speed);
        }
        else
        {
            GameObject obj = BattleManager.Instance.GetEnemy(myTran.position);
            if (obj != null) SetTarget(obj.transform);
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;
        if (targetSite != null)
        {
            GameObject obj = Instantiate(targetSite, target.position, Quaternion.identity);
            targetSiteTran = obj.transform;
            targetSiteTran.SetParent(target, true);
        }
    }

    public override void Break(bool isSpawn = true)
    {
        if (targetSiteTran != null)
        {
            Destroy(targetSiteTran.gameObject);
        }
        base.Break(isSpawn);
    }
}