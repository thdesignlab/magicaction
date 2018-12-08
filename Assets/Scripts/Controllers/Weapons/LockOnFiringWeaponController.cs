using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LockOnFiringWeaponController : FiringWeaponController
{
    protected List<LockOnBulletController> spawnList = new List<LockOnBulletController>();
    protected List<GameObject> enemies = new List<GameObject>();
    protected List<Transform> targets = new List<Transform>();

    //発射処理
    protected override IEnumerator Firing(InputStatus input)
    {
        spawnList = new List<LockOnBulletController>();
        targets = new List<Transform>();

        yield return StartCoroutine(base.Firing(input));

        foreach (LockOnBulletController lockOnCtrl in spawnList)
        {
            if (lockOnCtrl == null) continue;
            lockOnCtrl.Fire();
        }
    }

    //生成
    protected override GameObject Spawn(GameObject spawnObj, Vector2 pos, Quaternion qua)
    {
        GameObject obj = base.Spawn(spawnObj, pos, qua);
        LockOnBulletController ctrl = obj.GetComponent<LockOnBulletController>();
        spawnList.Add(ctrl);
        Transform target = GetTarget();
        ctrl.SetTarget(target);
        targets.Add(target);
        return obj;
    }

    //ターゲット取得
    protected Transform GetTarget()
    {
        Transform target = null;
        enemies.RemoveAll(o => o == null);

        if (enemies.Count <= 0)
        {
            enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag(Common.CO.TAG_ENEMY));
        }
        for (; ; )
        {
            if (enemies.Count <= 0) break;
            GameObject enemy = Common.FUNC.RandomList(enemies);
            if (enemy == null || targets.Contains(enemy.transform))
            {
                enemies.Remove(enemy);
                continue;
            }
            target = enemy.transform;
            break;
        }
        return target;
    }

    //ターゲット解除
    public void RemoveTarget(Transform t)
    {
        if (t == null) return;
        targets.Remove(t);
    }
}