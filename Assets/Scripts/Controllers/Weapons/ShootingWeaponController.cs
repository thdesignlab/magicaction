using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootingWeaponController : WeaponController
{
    [SerializeField]
    protected int rapidCount;
    [SerializeField]
    protected float rapidInterval;
    [SerializeField]
    protected float deviation;

    protected List<Transform> muzzules = new List<Transform>();

    protected override void Awake()
    {
        base.Awake();

        if (rapidCount <= 0) rapidCount = 1;
        if (rapidInterval < 0) rapidInterval = 0;
        SetMuzzles();
    }

    //発射位置設定
    protected void SetMuzzles()
    {
        foreach (Transform child in myTran)
        {
            if (child.tag != Common.CO.TAG_MUZZLE) continue;
            muzzules.Add(child);
        }
        if (muzzules.Count == 0) muzzules.Add(myTran);
    }

    //発射
    public override void Fire(InputStatus input)
    {
        base.Fire(input);

        UseMp();

        StartCoroutine(FireProcess(input.GetPoint()));
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