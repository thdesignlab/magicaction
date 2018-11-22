using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DirectionalityWeaponController : ShootingWeaponController
{

    //ターゲット取得
    protected override Vector2 GetTarget(InputStatus input)
    {
        Vector2 direction = input.GetStartPoint() - input.GetEndPoint();
        Vector2 target = direction + Common.FUNC.ParseVector2(myTran.position);
        return target;
    }
}