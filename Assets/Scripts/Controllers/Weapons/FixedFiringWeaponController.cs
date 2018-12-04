using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FixedFiringWeaponController : FiringWeaponController
{
    protected int defaultUseMp;
    protected List<GameObject> objList = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();

        defaultUseMp = useMp;
    }

    //発射処理
    protected override IEnumerator Firing(InputStatus input)
    {
        objList = new List<GameObject>();
        foreach (Transform muzzle in muzzles)
        {
            GameObject obj = Spawn(spawn, muzzle.position, muzzle.rotation);
            obj.transform.SetParent(muzzle, true);
            objList.Add(obj);
            yield return null;
        }
        float preTime = 0;
        float subMp = 0;
        for (; ; )
        {
            if (preTime >= input.pressTime) break;
            Vector2 targetPos = Common.FUNC.GetTargetWithDeviation(myTran.position, GetTarget(input), deviation);
            Common.FUNC.LookAt(myTran, targetPos);
            subMp += defaultUseMp * Time.deltaTime;
            if (subMp >= 1)
            {
                useMp = Mathf.FloorToInt(subMp);
                subMp -= useMp;
                UseMp();
            }
            preTime = input.pressTime;
            yield return null;
        }
        foreach (GameObject obj in objList)
        {
            if (obj == null) continue;
            obj.GetComponent<ObjectController>().Break();
        }
        fireCoroutine = null;
    }
}