using UnityEngine;
using System.Collections;

public class DisperseBulletController : BulletController
{
    [SerializeField]
    protected float speedDeviation;

    protected override void Start()
    {
        base.Start();
        float deviation = speedDeviation > 0 ? Common.FUNC.GetRandom(speedDeviation) : 0;
        SetSpeed(GetForward() * (speed + deviation));
    }
}