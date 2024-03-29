﻿using UnityEngine;
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
    protected Transform targetTran;

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
    public override GameObject Fire(InputStatus input)
    {
        index++;
        UseMp();
        Transform tran = muzzles.Count > 0 ? muzzles[0] : myTran;
        return Spawn(spawn, tran.position, tran.rotation, input.GetTapEnemyTran());
    }

    //生成
    protected virtual GameObject Spawn(GameObject spawnObj, Vector2 pos, Quaternion qua, Transform t = null)
    {
        if (spawnObj.tag == Common.CO.TAG_OBJECT)
        {
            if (!IsEnableSpawnPosition(pos)) return null;
        }

        GameObject obj = Instantiate(spawnObj, pos, qua);
        ObjectController objCtrl = obj.GetComponent<ObjectController>();
        objCtrl.SetPlayer(player);
        objCtrl.SetWeapon(this, index);
        objCtrl.SetTarget(t);
        return obj;
    }

    protected virtual bool IsEnableSpawnPosition(Vector3 pos)
    {
        //プレイヤー周辺
        if ((player.transform.position - pos).magnitude <= player.GetColliderRadius() * 1.5f) return false;
        //ボス周辺
        LayerMask mask = Common.FUNC.GetLayerMask(Common.CO.LAYER_ENEMY_BOSS);
        RaycastHit2D hit = Physics2D.CircleCast(pos, 3.0f, Vector2.zero, 0, mask);
        if (hit.collider != null) return false;
        //オブジェクト周辺
        mask = Common.FUNC.GetLayerMask(Common.CO.stageTags);
        hit = Physics2D.Raycast(pos, Vector2.zero, 0, mask);
        if (hit.collider != null) return false;
        return true;
    }
}