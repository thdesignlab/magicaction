using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpaceWeaponController : SpawnWeaponController
{
    //発射
    public override GameObject Fire(InputStatus input)
    {
        index++;
        UseMp();
        Transform tran = muzzles.Count > 0 ? muzzles[0] : myTran;
        return Spawn(spawn, input.GetPoint(), Quaternion.identity, input.GetTapEnemyTran());
    }
}