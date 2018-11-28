using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    protected float timeLimit;
    [SerializeField]
    protected GameObject spawnObj;

    protected Transform myTran;
    protected PlayerController player;
    protected WeaponController weapon;
    protected float deltaTime;
    protected float liveTime = 0;
    protected List<Transform> muzzles = new List<Transform>();

    protected const float LIMIT_AREA = 50.0f;
    protected const float LIMIT_TIME = 10.0f;

    protected virtual void Awake()
    {
        myTran = transform;
        player = GetComponent<PlayerController>();
        SetMuzzles();
        if (timeLimit <= 0 && !Common.FUNC.IsUnitTag(tag))
        {
            timeLimit = LIMIT_TIME;
        }
    }

    protected void SetMuzzles()
    {
        foreach (Transform child in myTran)
        {
            if (child.tag != Common.CO.TAG_MUZZLE) continue;
            muzzles.Add(child);
        }
    }

    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        deltaTime = Time.deltaTime;
        if (timeLimit > 0)
        {
            timeLimit -= deltaTime;
            if (timeLimit <= 0)
            {
                Break();
                return;
            }
        }
        if (Mathf.Abs(myTran.position.x) >= LIMIT_AREA || Mathf.Abs(myTran.position.y) >= LIMIT_AREA)
        {
            OutOfArea();
        }
    }

    protected virtual void OutOfArea()
    {
        Destroy(gameObject);
    }

    public virtual void Break()
    {
        if (spawnObj != null)
        {
            if (muzzles.Count > 0)
            {
                foreach (Transform muzzle in muzzles)
                {
                    Spawn(spawnObj, muzzle.position, muzzle.rotation);
                }
            }
            else
            {
                Spawn(spawnObj, myTran.position);
            }
        }
        Destroy(gameObject);
    }

    protected void Spawn(GameObject obj, Vector2 pos, Quaternion qua = default(Quaternion))
    {
        if (qua == default(Quaternion)) qua = Quaternion.identity;
        GameObject o = Instantiate(obj, pos, qua);
        SetParentCtrl(o);
    }

    public void SetParentCtrl(GameObject obj)
    {
        ObjectController objCtrl = obj.GetComponent<ObjectController>();
        objCtrl.SetPlayer(player);
        objCtrl.SetWeapon(weapon);
    }

    //### getter/setter ###

    public virtual void SetPlayer(PlayerController p)
    {
        player = p;
    }

    public virtual void SetWeapon(WeaponController w)
    {
        weapon = w;
    }

    public bool IsPlayer()
    {
        return (player != null);
    }
}