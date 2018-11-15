using UnityEngine;
using System.Collections;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private float timeLimit;
    [SerializeField]
    private GameObject spawnObj;

    protected Transform myTran;
    protected bool isPlayer;
    protected float deltaTime;
    protected float liveTime = 0;

    const float LIMIT_AREA = 30.0f;

    protected virtual void Awake()
    {
        myTran = transform;
        isPlayer = (tag == Common.CO.TAG_PLAYER);
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
            Destroy(gameObject);
            return;
        }
    }

    public virtual void Break()
    {
        if (spawnObj != null)
        {
            GameObject obj = Instantiate(spawnObj, myTran.position, myTran.rotation);
            obj.GetComponent<ObjectController>().SetPlayer(isPlayer);
        }
        Destroy(gameObject);
    }

    //### getter/setter ###

    public void SetPlayer(bool flg)
    {
        isPlayer = flg;
    }

    public bool IsPlayer()
    {
        return isPlayer;
    }
}