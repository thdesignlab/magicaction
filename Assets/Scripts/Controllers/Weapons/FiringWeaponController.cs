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

    //発射
    public override void Fire(InputStatus input)
    {
        if (fireCoroutine != null) return;
        index++;
        fireCoroutine = StartCoroutine(Firing(input));
    }

    //発射処理
    protected virtual IEnumerator Firing(InputStatus input)
    {
        float preTime = 0;
        for (; ; )
        {
            Debug.Log(preTime +" >> "+input.pressTime);
            if (preTime >= input.pressTime) break;
            Vector2 targetPos = Common.FUNC.GetTargetWithDeviation(myTran.position, GetTarget(input), deviation);
            Common.FUNC.LookAt(myTran, targetPos);
            foreach (Transform muzzle in muzzles)
            {
                Spawn(spawn, muzzle.position, muzzle.rotation);
                yield return null;
            }
            UseMp();
            preTime = input.pressTime;
            yield return new WaitForSeconds(rapidInterval);
        }
        fireCoroutine = null;
    }

}