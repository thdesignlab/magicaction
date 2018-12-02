using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootingWeaponController : SpawnWeaponController
{
    [SerializeField]
    protected int rapidCount;
    [SerializeField]
    protected float rapidInterval;
    [SerializeField]
    protected float deviation;

    protected override void Awake()
    {
        base.Awake();

        if (rapidCount <= 0) rapidCount = 1;
        if (rapidInterval < 0) rapidInterval = 0;
    }

    //発射
    public override void Fire(InputStatus input)
    {
        index++;
        UseMp();
        StartCoroutine(FireProcess(GetTarget(input)));
    }

    //発射処理
    protected IEnumerator FireProcess(Vector2 target)
    {
        for (int i = 0; i < rapidCount; i++)
        {
            Vector2 targetPos = Common.FUNC.GetTargetWithDeviation(myTran.position, target, deviation);
            Common.FUNC.LookAt(myTran, targetPos);
            foreach (Transform muzzle in muzzles)
            {
                Spawn(spawn, muzzle.position, muzzle.rotation);
                yield return null;
            }
            yield return new WaitForSeconds(rapidInterval);
        }
    }

}