using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectWeaponController : WeaponController
{
    [SerializeField]
    protected float minLength;
    [SerializeField]
    protected float maxLength;
    [SerializeField]
    protected GameObject multiSpawn;
    [SerializeField]
    protected int multiCount;
    [SerializeField]
    protected float multiInterval;

    //発射
    public override bool Fire(InputStatus input)
    {
        if (!base.Fire(input)) return false;

        LineRendererStatus lineStatus = new LineRendererStatus(input.linePositions);
        if (lineStatus.rate < 95)
        {
            StartCoroutine(ContinuousSpawn(input.GetStartPoint(), lineStatus.GetEvenlySpacedPoints(multiCount)));
        }
        else
        {
            Vector2 startPoint = input.GetStartPoint();
            Vector2 endPoint = input.GetEndPoint();
            Vector2 spawnPoint = (startPoint + endPoint) / 2;
            float length = (startPoint - endPoint).magnitude;
            if (length > maxLength)
            {
                length = maxLength;
            }
            else if (length < minLength)
            {
                length = minLength;
            }
            Quaternion q = Quaternion.LookRotation(Vector3.back, startPoint - endPoint);
            GameObject obj = Spawn(spawn, spawnPoint, q);
            obj.transform.localScale = new Vector2(1, length);
        }
        return true;
    }

    protected bool IsExistPlayer()
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
            GameObject obj = Spawn(multiSpawn, startPos + pos, Quaternion.identity);
            yield return new WaitForSeconds(multiInterval);
        }
    }
}