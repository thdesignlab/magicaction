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
    [SerializeField]
    protected int totalLimit;

    protected List<GameObject> objList = new List<GameObject>();

    //発射
    public override void Fire(InputStatus input)
    {
        if (totalLimit > 0) objList.RemoveAll(o => o == null);
        LineRendererStatus lineStatus = new LineRendererStatus(input.linePositions);
        StartCoroutine(ContinuousSpawn(input.GetStartPoint(), lineStatus.GetEvenlySpacedPoints(maxCount, perLength)));
    }

    IEnumerator ContinuousSpawn(Vector2 startPos, List<Vector2> posList)
    {
        int i = 0;
        foreach (Vector2 pos in posList)
        {
            GameObject obj = Spawn(spawn, startPos + pos, Quaternion.identity);
            if (obj != null) UseMp();
            if (totalLimit > 0)
            {
                objList.Add(obj);
                if (objList.Count > totalLimit)
                {
                    Destroy(objList[i++]);
                }
            }
            yield return new WaitForSeconds(multiInterval);
        }
    }
}