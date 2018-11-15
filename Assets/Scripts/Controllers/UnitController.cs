using UnityEngine;
using System.Collections;

public class UnitController : PhysicsController
{
    [SerializeField]
    protected int maxhp;

    protected int hp;

    protected override void Awake()
    {
        base.Awake();

        hp = maxhp;
        SetHp();
    }

    public virtual void Damage(int damage)
    {
        SetHp(-damage);
        if (hp <= 0) Dead();
    }

    protected virtual void Dead()
    {
        Break();
    }

    protected virtual void SetHp(int diff = 0)
    {
        hp += diff;
        if (hp > maxhp) hp = maxhp;
    }
}