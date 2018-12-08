using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBossController : EnemyController
{
    [SerializeField]
    protected List<GameObject> weaponList = new List<GameObject>();

    protected bool isReady = false;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(Summon());
    }

    protected override void Update()
    {
        if (!isReady) return;

        base.Update();

    }

    IEnumerator Summon()
    {
        OnChant(1, true);

        yield return new WaitForSeconds(1.5f);

        OnChant(1, false);
        isReady = true;
    }

    protected override void Assault(GameObject obj)
    {
        obj.GetComponent<UnitController>().Damage(strength);
    }

    protected override void OutOfArea()
    {
        return;
    }

    //### イベントハンドラ ###

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        //if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;
        //BreakableObjectController breakableObjCtrl = collision.gameObject.GetComponent<BreakableObjectController>();
        //if (breakableObjCtrl == null) return;
        base.OnCollisionEnter2D(collision);
    }
    protected override void OnCollisionStay2D(Collision2D collision)
    {
        //if (!Common.FUNC.IsStageTag(collision.gameObject.tag)) return;

        //foreach (ContactPoint2D contact in collision.contacts)
        //{
        //    Vector2 p = contact.point - Common.FUNC.ParseVector2(myTran.position);
        //    FlickFromStage(p);
        //}
        base.OnCollisionStay2D(collision);
    }
}
