using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyWeaponController : WeaponController
{
    [SerializeField]
    protected GameObject spawn;
    [SerializeField]
    protected float rapidCount;
    [SerializeField]
    protected float rapidInterval;

    protected List<Transform> muzzles = new List<Transform>();
    protected List<GameObject> objs = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();

        SetMuzzles();
        rapidCount = (rapidCount <= 0) ? 1 : rapidCount;
    }

    //発射位置設定
    protected void SetMuzzles()
    {
        foreach (Transform child in myTran)
        {
            if (child.tag != Common.CO.TAG_MUZZLE) continue;
            muzzles.Add(child);
        }
        if (muzzles.Count == 0) muzzles.Add(myTran);
    }

    //発射
    public override GameObject Fire(InputStatus input)
    {
        StartCoroutine(RapidFire());
        return null;
    }
    protected IEnumerator RapidFire()
    {
        for (int i = 0; i < rapidCount; i++ )
        {
            foreach (Transform muzzle in muzzles)
            {
                GameObject obj = Spawn(spawn, muzzle.position, muzzle.rotation);
                objs.Add(obj);
                yield return null;
            }
            yield return new WaitForSeconds(rapidInterval);
        }
    }

    //生成
    protected virtual GameObject Spawn(GameObject spawnObj, Vector2 pos, Quaternion qua)
    {
        GameObject obj = Instantiate(spawnObj, pos, qua);
        return obj;
    }

    //生成オブジェクト
    public List<GameObject> GetExistObject()
    {
        objs.RemoveAll(o => o == null);
        return objs;
    }
        
}