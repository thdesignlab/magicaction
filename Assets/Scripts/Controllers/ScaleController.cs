using UnityEngine;
using System.Collections;

public class ScaleController : MonoBehaviour
{
    protected Transform myTran;

    [SerializeField]
    private Vector3 startScale;
    [SerializeField]
    private Vector3 endScale;
    [SerializeField]
    private float scaleTime;
    [SerializeField]
    private float scaleLateTime;
    [SerializeField]
    private float continuationTime;

    private float activeTime = 0;
    private bool isReverse = false;

    protected void Awake()
    {
        myTran = transform;
    }
	
	void Update ()
    {
        if (!isReverse)
        {
            if (scaleTime <= 0 || activeTime > scaleTime + scaleLateTime) return;

            activeTime += Time.deltaTime;

            if (scaleLateTime > 0 && scaleLateTime >= activeTime) return;

            float rate = (activeTime - scaleLateTime) / scaleTime;
            ChangeScale(Vector3.Lerp(startScale, endScale, rate));
            if (rate >= 1)
            {
                isReverse = true;
                activeTime = 0;
            }
        }
        else
        {
            if (continuationTime == 0 || activeTime > scaleTime + continuationTime) return;

            activeTime += Time.deltaTime;

            if (continuationTime >= activeTime) return;

            float rate = (activeTime - continuationTime) / scaleTime;
            ChangeScale(Vector3.Lerp(endScale, startScale, rate));
        }
    }

    void OnEnable()
    {
        ChangeScale(startScale);
        activeTime = 0;
    }

    private void ChangeScale(Vector3 scale)
    {
        myTran.localScale = scale;
    }
}
