using UnityEngine;
using System.Collections;

public class LockOnBulletController : DamageObjectController
{
    [SerializeField]
    protected float firstSpeed;
    [SerializeField]
    protected float atackSpeed;
    [SerializeField]
    protected float stopTime;
    [SerializeField]
    protected float randomSpeed;
    [SerializeField]
    protected float randomTime;

    protected bool isFire = false;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(FirstMove());
    }

    IEnumerator FirstMove()
    {
        SetSpeed(GetForward() * firstSpeed);
        stopTime += Common.FUNC.GetRandom(randomTime);
        for (; ; )
        {
            stopTime -= Time.deltaTime;
            liveTime = 0;
            if (stopTime <= 0) break;
            yield return null;
        }
        if (isFire) yield break;
        SetSpeed(Vector2.zero);
    }

    public void Fire(Vector2 target)
    {
        isFire = true;
        Common.FUNC.LookAt(myTran, target - (Vector2)myTran.position);
        SetSpeed(GetForward() * (atackSpeed + Common.FUNC.GetRandom(randomSpeed)));
    }
    public void Fire()
    {
        Vector2 rand = new Vector2(Common.FUNC.GetRandom(30), Common.FUNC.GetRandom(30));
        Fire(rand);
    }
}