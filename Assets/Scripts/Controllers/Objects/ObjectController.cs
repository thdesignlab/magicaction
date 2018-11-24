using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private float timeLimit;
    [SerializeField]
    private GameObject spawnObj;

    protected Transform myTran;
    protected PlayerController player;
    protected float deltaTime;
    protected float liveTime = 0;
    protected List<Transform> muzzles = new List<Transform>();

    const float LIMIT_AREA = 50.0f;

    protected virtual void Awake()
    {
        myTran = transform;
        player = GetComponent<PlayerController>();
        SetMuzzles();
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
                    GameObject obj = Instantiate(spawnObj, muzzle.position, muzzle.rotation);
                    obj.GetComponent<ObjectController>().SetPlayer(player);
                }
            }
            else
            {
                GameObject obj = Instantiate(spawnObj, myTran.position, Quaternion.identity);
                obj.GetComponent<ObjectController>().SetPlayer(player);
            }
        }
        Destroy(gameObject);
    }

    //### getter/setter ###

    public virtual void SetPlayer(PlayerController p)
    {
        player = p;
    }

    public bool IsPlayer()
    {
        return (player != null);
    }
}