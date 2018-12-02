using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnWeaponController : WeaponController
{
    [SerializeField]
    protected GameObject spawn;
    [SerializeField]
    protected int useMp;

    protected List<Transform> muzzles = new List<Transform>();
    protected Camera _mainCam;
    protected Camera mainCam { get { return _mainCam ? _mainCam : _mainCam = Camera.main; } }

    protected override void Awake()
    {
        base.Awake();

        SetMuzzles();
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

    //MP消費
    protected bool UseMp()
    {
        if (!player) return true;

        if (!player.UseMp(useMp)) return false;

        return true;
    }

    //発射
    public override void Fire(InputStatus input)
    {
        index++;
        UseMp();
        Transform tran = muzzles.Count > 0 ? muzzles[0] : myTran;
        Spawn(spawn, tran.position, tran.rotation);
    }

    //生成
    protected GameObject Spawn(GameObject spawnObj, Vector2 pos, Quaternion qua)
    {
        if (spawnObj.tag == Common.CO.TAG_OBJECT)
        {
            if (!IsEnableSpawnPosition(pos)) return null;
        }

        GameObject obj = Instantiate(spawnObj, pos, qua);
        ObjectController objCtrl = obj.GetComponent<ObjectController>();
        objCtrl.SetPlayer(player);
        objCtrl.SetWeapon(this, index);
        return obj;
    }

    protected virtual bool IsEnableSpawnPosition(Vector3 pos)
    {
        return ((player.transform.position - pos).magnitude > player.GetColliderRadius() * 1.5f);
    }
}