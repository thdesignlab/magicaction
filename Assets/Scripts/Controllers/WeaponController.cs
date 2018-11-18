using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    [SerializeField]
    protected GameObject bullet;
    [SerializeField]
    protected int rapidCount;
    [SerializeField]
    protected float rapidInterval;
    [SerializeField]
    protected float deviation;
    [SerializeField]
    protected int useMp;

    protected Transform myTran;
    protected List<Transform> muzzules = new List<Transform>();
    protected PlayerController player;

    private void Awake()
    {
        myTran = transform;
        if (rapidCount <= 0) rapidCount = 1;
        if (rapidInterval < 0) rapidInterval = 0;
        SetMuzzles();
    }

    //発射位置設定
    private void SetMuzzles()
    {
        foreach (Transform child in myTran)
        {
            if (child.tag != Common.CO.TAG_MUZZLE) continue;
            muzzules.Add(child);
        }
        if (muzzules.Count == 0) muzzules.Add(myTran);
    }

    //Player設定
    public void SetPlayer(PlayerController p)
    {
        player = p;
    }

    //MP消費
    protected bool UseMp(int level = 0)
    {
        if (!player) return true;

        if (!player.UseMp(useMp)) return false;

        return true;
    }


    //発射
    public bool Fire(Vector2 target, int level = 0)
    {
        if (!UseMp(level)) return false;
            
        StartCoroutine(FireProcess(target));

        return true;
    }

    //発射処理
    protected IEnumerator FireProcess(Vector2 target)
    {
        for (int i = 0; i < rapidCount; i++)
        {
            Common.FUNC.LookAt(myTran, target + GetDeviation());
            foreach (Transform muzzule in muzzules)
            {
                Spawn(muzzule, bullet);
                yield return null;
            }
            yield return new WaitForSeconds(rapidInterval);
        }
    }

    //弾生成
    protected void Spawn(Transform muzzle, GameObject spawnObj)
    {
        GameObject obj = Instantiate(spawnObj, muzzle.position, muzzle.rotation);
        obj.GetComponent<ObjectController>().SetPlayer(true);

    }

    //誤差取得
    protected Vector2 GetDeviation()
    {
        if (deviation == 0) return Vector2.zero;
        return new Vector2(Common.FUNC.GetRandom(deviation), Common.FUNC.GetRandom(deviation));

    }
}