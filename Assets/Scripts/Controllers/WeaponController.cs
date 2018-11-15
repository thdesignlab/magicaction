using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    protected GameObject bullet;

    protected List<Transform> muzzules = new List<Transform>();

    private void Start()
    {
        
    }

    public void Fire()
    {
    }
}