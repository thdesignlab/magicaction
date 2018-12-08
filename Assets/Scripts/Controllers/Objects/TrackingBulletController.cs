using UnityEngine;
using System.Collections;

public class TrackingBulletController : BulletController
{
    [SerializeField]
    protected float turnAngle;
    [SerializeField]
    protected GameObject targetSite;

    protected Transform targetSiteTran;

    protected override void Update()
    {
        base.Update();

        if (targetTran != null)
        {
            Common.FUNC.LookAt(myTran, targetTran.position, turnAngle * Time.deltaTime);
            SetSpeed(GetForward() * speed);
        }
        else
        {
            GameObject obj = BattleManager.Instance.GetEnemy(myTran.position);
            if (obj != null) SetTarget(obj.transform);
        }
    }

    public override void SetTarget(Transform t)
    {
        base.SetTarget(t);

        if (targetTran == null) return;

        if (targetSite != null)
        {
            GameObject obj = Instantiate(targetSite, targetTran.position, Quaternion.identity);
            targetSiteTran = obj.transform;
            targetSiteTran.SetParent(targetTran, true);
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