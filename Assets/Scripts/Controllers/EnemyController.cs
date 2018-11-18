using UnityEngine;
using System.Collections;

public class EnemyController : UnitController
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private float attackInterval;
    [SerializeField]
    private float rapidCount;
    private float nextAttackTime;

    protected GameObject player;

    protected override void Awake()
    {
        base.Awake();

        rapidCount = (rapidCount > 0) ? rapidCount: 1;
        nextAttackTime = attackInterval;
    }

    protected override void Update()
    {
        base.Update();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag(Common.CO.TAG_PLAYER);
        }
        nextAttackTime -= deltaTime;
        if (attackInterval > 0 && nextAttackTime <= 0)
        {
            Vector2 targetPos = (player != null) ? player.transform.position : myTran.position;
            StartCoroutine(Rapid(targetPos));
            nextAttackTime = attackInterval;
        }
    }
    protected override void InitSpeed()
    {
        base.InitSpeed();

        SetSpeed(GetForward() * speed);
    }

    IEnumerator Rapid(Vector2 targetPos)
    {
        float diff = 1.0f;
        for (int i = 0; i < rapidCount; i++)
        {
            Vector2 target = targetPos + new Vector2(Random.Range(-diff, diff), Random.Range(-diff, diff));
            Spawn(bullet, targetPos);
            yield return new WaitForSeconds(0.1f);
        }
    }
    private void Spawn(GameObject spawnObj, Vector2 target)
    {
        GameObject obj = Instantiate(spawnObj, myTran.position + new Vector3(1, 0, 0), Quaternion.identity);
        LookAt(obj.transform, new Vector3(target.x, target.y, obj.transform.position.z));
    }

    protected override void Dead()
    {
        BattleManager.Instance.AddScore();
        base.Dead();
    }

    protected void Assault(GameObject obj)
    {
        int d = (strength == 0) ? hp * hp : hp * strength;
        GameObject spawn = Resources.Load<GameObject>("Effects/Assault");
        obj.GetComponent<UnitController>().Damage(d);
        if (spawn != null) Instantiate(spawn, myTran.position, Quaternion.identity);
        base.Dead();
    }

    //### イベントハンドラ ###

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);

        if (collision.gameObject.tag != Common.CO.TAG_PLAYER) return;

        Assault(collision.gameObject);
    }
}