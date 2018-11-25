using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineWeaponController : SpawnWeaponController
{
    [SerializeField]
    protected int maxCount;
    [SerializeField]
    protected float perLength;
    [SerializeField]
    protected float multiInterval;
    [SerializeField]
    protected float spawnAngle;

    //発射
    public override void Fire(InputStatus input)
    {
        LineRendererStatus lineStatus = new LineRendererStatus(input.linePositions);
        StartCoroutine(ContinuousSpawn(input.GetStartPoint(), lineStatus.GetEvenlySpacedPoints(maxCount, perLength)));
    }

    IEnumerator ContinuousSpawn(Vector2 startPos, List<Vector2> posList)
    {
        foreach (Vector2 pos in posList)
        {
            GameObject obj = Spawn(spawn, startPos + pos, Quaternion.identity);
            if (obj != null) UseMp();
            yield return new WaitForSeconds(multiInterval);
        }
    }
}