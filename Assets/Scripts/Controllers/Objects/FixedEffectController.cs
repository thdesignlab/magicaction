using UnityEngine;
using System.Collections;

public class FixedEffectController : DamageObjectController
{
    [SerializeField]
    protected int useMp;

    private float ump = 0;

    public override void SetWeapon(WeaponController w, int i)
    {
        base.SetWeapon(w, i);

        if (w != null)
        {
            myTran.SetParent(w.transform, true);
        }
    }

    protected override void Update()
    {
        base.Update();

        //MP消費
        if (player != null)
        {
            ump += useMp * deltaTime;
            if (ump >= 1.0f)
            {
                int r = (int)Mathf.Floor(ump);
                player.UseMp(r);
                ump -= r;
            }
        }
    }

}