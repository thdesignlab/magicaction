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
    protected int totalLimit;
    [SerializeField, HeaderAttribute("Angle")]
    protected bool isPlayerForward;
    [SerializeField]
    protected bool isLineAngle;

    private void OnValidate()
    {
        if (isPlayerForward)
        {
            isLineAngle = false;
        }
        if (isLineAngle)
        {
            isPlayerForward = false;
        }
    }

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
            if (obj != null)
            {
                UseMp();
                if (isPlayerForward || isLineAngle)
                {
                    Vector3 target = Vector3.zero;
                    if (isPlayerForward)
                    {
                        target = obj.transform.position + (obj.transform.position - player.transform.position);
                    }
                    else if (isLineAngle)
                    {
                        Vector2 objPos = obj.transform.position;
                        if (i == 0)
                        {
                            target = objPos + posList[i + 1] - pos;
                        }
                        else
                        {
                            target = objPos + pos - posList[i - 1];
                        }
                    }
                    Common.FUNC.LookAt(obj.transform, target);
                }
                if (totalLimit > 0)
                {
                    objList.Add(obj);
                    if (objList.Count > totalLimit)
                    {
                        Destroy(objList[i++]);
                    }
                }
            }
            i++;
            yield return new WaitForSeconds(multiInterval);
        }
    }
}