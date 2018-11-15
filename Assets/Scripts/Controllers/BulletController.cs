using UnityEngine;
using System.Collections;

public class BulletController : DamageObjectController
{
    [SerializeField]
    protected int speed;

    protected override void Update()
    {
        base.Update();
        MoveForward(speed);
    }
}