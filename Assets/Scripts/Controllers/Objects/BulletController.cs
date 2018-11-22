using UnityEngine;
using System.Collections;

public class BulletController : DamageObjectController
{
    [SerializeField]
    protected int speed;

    protected override void Start()
    {
        base.Start();
        SetSpeed(GetForward() * speed);
    }
}