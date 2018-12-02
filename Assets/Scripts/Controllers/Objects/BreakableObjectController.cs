using UnityEngine;
using System.Collections;

public class BreakableObjectController : PhysicsController
{
    [SerializeField]
    private GameObject breakSpawnObject;

    public override void Break(bool isSpawn = true)
    {
        if (strength <= 0)
        {
            spawnObj = breakSpawnObject;
        }
        base.Break(isSpawn);
    }
}