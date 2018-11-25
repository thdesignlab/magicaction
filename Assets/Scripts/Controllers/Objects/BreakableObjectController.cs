using UnityEngine;
using System.Collections;

public class BreakableObjectController : PhysicsController
{
    [SerializeField]
    protected int useMp;

    private float ump = 0;

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