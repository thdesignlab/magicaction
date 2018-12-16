using UnityEngine;
using System.Collections;

public class BreakableObjectController : PhysicsController
{
    [SerializeField]
    private GameObject breakSpawnObject;
    [SerializeField]
    private float securityTime;

    protected override void Update()
    {
        base.Update();

        if (securityTime > 0) securityTime -= deltaTime;
    }

    public override void Break(bool isSpawn = true)
    {
        if (strength <= 0)
        {
            spawnObj = breakSpawnObject;
        }
        base.Break(isSpawn);
    }

    //耐久値削減
    public override void Scrape(int power = 0)
    {
        if (power < 0 || securityTime > 0) return;

        strength -= (power == 0) ? strength : power;
        if (strength <= 0)
        {
            Break();
        }
    }
}