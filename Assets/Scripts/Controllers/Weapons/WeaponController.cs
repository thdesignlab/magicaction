using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    protected GameObject spawn;
    [SerializeField]
    protected int useMp;

    protected Transform myTran;
    protected PlayerController player;

    protected virtual void Awake()
    {
        myTran = transform;
    }

    //Player設定
    public void SetPlayer(PlayerController p)
    {
        player = p;
    }

    //MP消費
    protected bool UseMp()
    {
        if (!player) return true;

        if (!player.UseMp(useMp)) return false;

        return true;
    }

    //生成
    protected virtual GameObject Spawn(GameObject spawnObj, Vector2 pos, Quaternion qua)
    {
        GameObject obj = Instantiate(spawnObj, pos, qua);
        obj.GetComponent<ObjectController>().SetPlayer(true);
        return obj;
    }

    //発射
    public virtual bool Fire(InputStatus input)
    {
        if (!UseMp()) return false;

        return true;
    }
}