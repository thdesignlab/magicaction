using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlexibleWeaponController : WeaponController
{
    [SerializeField]
    protected GameObject upwardWeapon;
    [SerializeField]
    protected GameObject frontWeapon;
    [SerializeField]
    protected GameObject underWeapon;
    [SerializeField]
    protected float borderAngle = 35;
    [SerializeField]
    protected bool isDrag;

    protected WeaponController upwardWeaponCtrl;
    protected WeaponController frontWeaponCtrl;
    protected WeaponController underWeaponCtrl;

    //Player設定
    public override void SetPlayer(PlayerController p)
    {
        base.SetPlayer(p);
        upwardWeaponCtrl = player.EquipWeapon(upwardWeapon);
        frontWeaponCtrl = player.EquipWeapon(frontWeapon);
        underWeaponCtrl = player.EquipWeapon(underWeapon);
    }

    //発射
    public override void Fire(InputStatus input)
    {
        Vector2 start = input.GetStartPoint();
        Vector2 end = input.GetEndPoint();
        float borderAngleAbs = Mathf.Abs(borderAngle);
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        float rad = Mathf.Atan2(dy, dx);
        float angle = rad * Mathf.Rad2Deg;

        if (Mathf.Abs(angle) <= borderAngleAbs || 180 - Mathf.Abs(angle) <= borderAngleAbs)
        {
            if (frontWeaponCtrl != null) frontWeaponCtrl.Fire(input);
        }
        else
        {
            if ((angle > 0 && !isDrag) || (angle < 0 && isDrag))
            {
                if (upwardWeaponCtrl != null) upwardWeaponCtrl.Fire(input);
            }
            else
            {
                if (underWeaponCtrl != null) underWeaponCtrl.Fire(input);
            }
        }
    }

}