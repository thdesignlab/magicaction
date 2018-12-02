using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ObjectWeaponController : SpawnWeaponController
{
    [SerializeField]
    protected float minLength;
    [SerializeField]
    protected float maxLength;

    //発射
    public override void Fire(InputStatus input)
    {
        index++;
        UseMp();

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
}