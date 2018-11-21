using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LineWeaponController : WeaponController
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
    public override bool Fire(InputStatus input)
    {
        if (!base.Fire(input)) return false;

        LineRendererStatus lineStatus = new LineRendererStatus(input.linePositions);
        StartCoroutine(ContinuousSpawn(input.GetStartPoint(), lineStatus.GetEvenlySpacedPoints(maxCount, perLength)));
        return true;
    }

    protected bool IsSpawnPosition()
    {
        //if (player != null)
        //{
        //    foreach (Collider2D col in Physics2D.OverlapCircleAll(player.transform.position, player.GetColliderRadius()))
        //    {
        //        Debug.Log(col.name);
        //    }
        //}
        return false;
    }

    IEnumerator ContinuousSpawn(Vector2 startPos, List<Vector2> posList)
    {
        foreach (Vector2 pos in posList)
        {
            GameObject obj = Spawn(spawn, startPos + pos, Quaternion.identity);
            yield return new WaitForSeconds(multiInterval);
        }
    }
}