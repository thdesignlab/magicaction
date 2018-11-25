using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    protected Transform myTran;
    protected PlayerController player;

    protected virtual void Awake()
    {
        myTran = transform;
    }

    //Player設定
    public virtual void SetPlayer(PlayerController p)
    {
        player = p;
    }

    //発射
    public virtual void Fire(InputStatus input)
    {
    }

    //目標ポイント取得
    protected virtual Vector2 GetTarget(InputStatus input)
    {
        return input.GetPoint();
    }
}