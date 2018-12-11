using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShootingEnemyWeaponController : EnemyWeaponController
{
    //発射
    public override GameObject Fire(InputStatus input)
    {
        StartCoroutine(RapidFire(input.point));
        return null;
    }
}