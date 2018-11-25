using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShooterObjectController : ObjectController
{
    [SerializeField, Range(0, 0)]
    private int targetType;
    [SerializeField]
    private int spawnCount;
    [SerializeField]
    private float spawnInterval;
    [SerializeField]
    private Vector2 deviation;

    const int TARGET_TYPE_ZERO = 0;

    public override void Break()
    {
        if (spawnObj != null)
        {
            StartCoroutine(spawnProcess());
        }
        else
        {
            Destroy(gameObject);

        }
    }

    IEnumerator spawnProcess()
    {
        Vector2 targetPos = GetTarget();
        for (int i = 0; i < spawnCount; i++)
        {
            Vector2 diff = GetDeviation();
            Common.FUNC.LookAt(myTran, targetPos + diff);
            if (muzzles.Count > 0)
            {
                foreach (Transform muzzle in muzzles)
                {
                    Spawn(spawnObj, muzzle.position, muzzle.rotation);
                }
            }
            else
            {
                Spawn(spawnObj, myTran.position, myTran.rotation);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
        Destroy(gameObject);
    }

    protected Vector2 GetTarget()
    {
        Vector2 target = Vector2.zero;
        switch (targetType)
        {
            case TARGET_TYPE_ZERO:
                break;
        }
        return target;
    }

    protected Vector2 GetDeviation()
    {
        Vector2 diff = Vector2.zero;
        if (deviation != Vector2.zero)
        {
            diff.x = Common.FUNC.GetRandom(deviation.x);
            diff.y = Common.FUNC.GetRandom(deviation.y);
        }
        return diff;
    }
}