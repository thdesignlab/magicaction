using UnityEngine;
using System.Collections;

public class FixedBreakableObjectController : BreakableObjectController
{
    [SerializeField]
    protected int useMp;

    private float ump = 0;

    public override void SetPlayer(PlayerController p)
    {
        base.SetPlayer(p);

        if (player != null)
        {
            myTran.SetParent(player.transform, true);
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