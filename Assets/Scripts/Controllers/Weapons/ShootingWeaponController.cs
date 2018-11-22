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
        UseMp();

        StartCoroutine(FireProcess(GetTarget(input)));
    }

    //発射処理
    protected IEnumerator FireProcess(Vector2 target)
    {
        for (int i = 0; i < rapidCount; i++)
        {
            Common.FUNC.LookAt(myTran, target + GetDeviation());
            foreach (Transform muzzule in muzzules)
            {
                Spawn(spawn, muzzule.position, muzzule.rotation);
                yield return null;
            }
            yield return new WaitForSeconds(rapidInterval);
        }
    }

    //誤差取得
    protected Vector2 GetDeviation()
    {
        if (deviation == 0) return Vector2.zero;
        return new Vector2(Common.FUNC.GetRandom(deviation), Common.FUNC.GetRandom(deviation));

    }
}