using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    [SerializeField]
    private Vector2 spawnMin = -Vector2.one;
    [SerializeField]
    private Vector2 spawnMax = Vector2.one;
    [SerializeField]
    private bool isEndless;

    protected override void Awake()
    {
        isDontDestroyOnLoad = false;
        base.Awake();
    }

    public Vector2 GetRandomPoint(bool isGround = false)
    {
        float x = Random.Range(spawnMin.x, spawnMax.x);
        x = (x >= 0) ? Mathf.Ceil(x) : Mathf.Floor(x);
        if (x == 0) x = 1;
        float y = isGround ? -1 : Random.Range(spawnMin.y, spawnMax.y);
        return GetPoint(new Vector2(x, y));
    }

    public Vector2 GetPoint(Vector2 v, bool isAbsolute = true)
    {
        if (isAbsolute)
        {
            v.x = Mathf.Clamp(v.x, spawnMin.x, spawnMax.x);
            v.y = Mathf.Clamp(v.y, spawnMin.y, spawnMax.y);
        }
        Vector2 min = ScreenManager.Instance.GetScreenMinPos();
        Vector2 max = ScreenManager.Instance.GetScreenMaxPos();
        Vector2 center = (max + min) / 2;
        Vector2 length = (max - min) / 2;
        Vector2 pos = new Vector2(length.x * v.x, length.y * v.y);
        if (isAbsolute) pos += center;
        return pos;
    }

    public Quaternion GetQuaternion(Vector2 v)
    {
        Quaternion qua = Quaternion.identity;
        if (v.x > 0) qua.eulerAngles = Vector2.up * 180;
        return qua;
    }

    public bool IsEndless()
    {
        return isEndless;
    }
}