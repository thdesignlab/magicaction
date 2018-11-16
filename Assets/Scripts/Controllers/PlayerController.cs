using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : UnitController
{
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private GameObject arrow;
    [SerializeField]
    private GameObject laser;
    [SerializeField]
    private GameObject spread;

    [SerializeField]
    private Slider hpSlider;

    private float recover = 0;

    protected override void Update()
    {
        base.Update();

        recover += 10 * deltaTime;
        if (recover >= 1.0f)
        {
            int r = (int)Mathf.Floor(recover);
            SetHp(r);
            recover -= r;
        }
    }

    protected override void SetHp(int diff)
    {
        base.SetHp(diff);

        hpSlider.value = (float)hp / maxhp;
    }

    public void Drag(InputStatus status)
    {
        StartCoroutine(Rapid(status));
    }
    IEnumerator Rapid(InputStatus status)
    {
        for (int i = 0; i <= 5; i++)
        {
            Spawn(arrow, status.point);
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Shield(InputStatus status)
    {

    }

    public void Arrow(InputStatus status)
    {
        StartCoroutine(Spread(status));
    }
    IEnumerator Spread(InputStatus status)
    {
        float diff = 1.0f;
        for (int i = 0; i <= 5; i++)
        {
            Vector2 target = status.point + new Vector2(Random.Range(-diff, diff), Random.Range(-diff, diff));
            Spawn(spread, target);
            yield return null;
        }
    }

    public void Shoot(InputStatus status)
    {
        Spawn(bullet, status.point);
    }

    public void Laser(InputStatus status)
    {
        Spawn(laser, status.point);
    }

    private void Spawn(GameObject spawnObj, Vector2 target)
    {
        GameObject obj = Instantiate(spawnObj, myTran.position + new Vector3(1, 0, 0), Quaternion.identity);
        LookAt(obj.transform, new Vector3(target.x, target.y, obj.transform.position.z));
        obj.GetComponent<ObjectController>().SetPlayer(true);
    }




    public void Rain(InputStatus status)
    {
        Spawn(bullet, myTran.position + Vector3.up + Vector3.right);
        StartCoroutine(Meteor());
    }
    IEnumerator Meteor()
    {
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i <= 100; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-30, 30), 20, myTran.position.z);
            float diff = Random.Range(-3, 3);
            GameObject obj = Instantiate(bullet, pos, Quaternion.identity);
            LookAt(obj.transform, new Vector3(pos.x + diff, pos.y - 10, myTran.position.z));
            obj.GetComponent<ObjectController>().SetPlayer(true);
            yield return new WaitForSeconds(0.04f);
        }
    }
}