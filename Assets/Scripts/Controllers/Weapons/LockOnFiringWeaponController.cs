using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockOnFiringWeaponController : FiringWeaponController
{
    protected List<LockOnBulletController> spawnList = new List<LockOnBulletController>();

    //発射処理
    protected override IEnumerator Firing(InputStatus input)
    {
        spawnList = new List<LockOnBulletController>();
        yield return StartCoroutine(base.Firing(input));

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(Common.CO.TAG_ENEMY);
        int i = 1;
        foreach (LockOnBulletController lockOnCtrl in spawnList)
        {
            if (lockOnCtrl == null) continue;
            
            if (i % 2 == 0 && enemies.Length > i / 2)
            {
                lockOnCtrl.Fire(enemies[i / 2].transform.position);
            }
            else
            {
                lockOnCtrl.Fire();
            }
        }
    }

    //生成
    protected override GameObject Spawn(GameObject spawnObj, Vector2 pos, Quaternion qua)
    {
        GameObject obj = base.Spawn(spawnObj, pos, qua);
        spawnList.Add(obj.GetComponent<LockOnBulletController>());
        return obj;
    }
}