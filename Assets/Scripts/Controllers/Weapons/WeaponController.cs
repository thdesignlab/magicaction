using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    protected bool isDrag;
    protected Transform myTran;
    protected PlayerController player;
    protected WeaponResult weaponResult;
    protected int index = -1;

    protected virtual void Awake()
    {
        myTran = transform;
        weaponResult = new WeaponResult(name);
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
        Vector2 target = input.GetPoint();
        if (isDrag)
        {
            Vector2 direction = input.GetStartPoint() - input.GetEndPoint();
            target = direction + Common.FUNC.ParseVector2(myTran.position);
        }
        return target;
    }

    //ダメージ情報追加
    public void AddDamageWR(int i, float damage, bool isKill = false)
    {
        weaponResult.AddDamage(i, damage, isKill);
    }

}