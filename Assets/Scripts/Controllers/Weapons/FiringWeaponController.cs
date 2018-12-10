using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FiringWeaponController : SpawnWeaponController
{
    [SerializeField]
    protected float rapidInterval;
    [SerializeField]
    protected float deviation;

    protected Coroutine fireCoroutine;
    protected float interval = 0;

    //発射
    public override GameObject Fire(InputStatus input)
    {
        if (fireCoroutine != null) return null;
        index++;
        fireCoroutine = StartCoroutine(Firing(input));
        return null;
    }

    //発射処理
    protected virtual IEnumerator Firing(InputStatus input)
    {
        float preTime = 0;
        interval = 0;
        for (; ; )
        {
            for (; ; )
            {
                if (preTime >= input.pressTime) goto FIRE;
                preTime = input.pressTime;
                interval -= Time.deltaTime;
                if (interval <= 0) break;
                yield return null;
            }

            Vector2 targetPos = Common.FUNC.GetTargetWithDeviation(myTran.position, GetTarget(input), deviation);
            Common.FUNC.LookAt(myTran, targetPos);
            foreach (Transform muzzle in muzzles)
            {
                Spawn(spawn, muzzle.position, muzzle.rotation);
                yield return null;
            }
            UseMp();
            interval = rapidInterval;

        }
        FIRE:
        fireCoroutine = null;
    }

}