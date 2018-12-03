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
        Debug.Log("Firing");
        objList = new List<GameObject>();
        foreach (Transform muzzle in muzzles)
        {
            GameObject obj = Spawn(spawn, muzzle.position, muzzle.rotation);
            obj.transform.SetParent(muzzle, true);
            objList.Add(obj);
            yield return null;
        }
        float subMp = 0;
        for (; ; )
        {
            if (input.isReset) break;
            Vector2 targetPos = Common.FUNC.GetTargetWithDeviation(myTran.position, GetTarget(input), deviation);
            Common.FUNC.LookAt(myTran, targetPos);
            subMp += defaultUseMp * Time.deltaTime;
            if (subMp >= 1)
            {
                useMp = Mathf.FloorToInt(subMp);
                subMp -= useMp;
                UseMp();
            }
            yield return null;
        }
        //foreach (GameObject obj in objList)
        //{
        //    if (obj == null) continue;
        //    obj.GetComponent<ObjectController>().Break();
        //}
        Debug.Log("break");
        fireCoroutine = null;
    }
}