using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawingWeaponController : SpawnWeaponController
{
    [SerializeField]
    protected float perLength;
    [SerializeField]
    protected int totalLimit;
    [SerializeField, HeaderAttribute("Angle")]
    protected bool isPlayerForward;
    [SerializeField]
    protected bool isLineAngle;

    protected Coroutine fireCoroutine;

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
        if (fireCoroutine != null) return;
        index++;
        fireCoroutine = StartCoroutine(ContinuousSpawn(input));
    }

    IEnumerator ContinuousSpawn(InputStatus input)
    {
        float preTime = 0;
        float sumDistance = 0;
        for (; ; )
        {
            if (preTime >= input.pressTime) break;
            Vector2 pos = input.GetPoint();
            Vector2 prePos = input.GetPrePoint();
            sumDistance += (pos - prePos).magnitude;
            if (sumDistance >= perLength)
            {
                sumDistance = 0;
                if (totalLimit > 0) objList.RemoveAll(o => o == null);
                GameObject obj = Spawn(spawn, pos, Quaternion.identity);
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
                            target = pos + pos - prePos;
                        }
                        Common.FUNC.LookAt(obj.transform, target);
                    }
                    if (totalLimit > 0)
                    {
                        objList.Add(obj);
                        if (objList.Count > totalLimit)
                        {
                            Destroy(objList[0]);
                        }
                    }
                }
            }
            preTime = input.pressTime;
            yield return null;
        }
        fireCoroutine = null;
    }
}