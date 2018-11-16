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

        MoveForward(speed);

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
}