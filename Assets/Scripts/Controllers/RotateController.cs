using UnityEngine;
using System.Collections;

public class RotateController : MonoBehaviour
{
    protected Transform myTran;

    [SerializeField]
    private Vector3 angle;
    [SerializeField]
    private float angleTime;
    [SerializeField]
    private bool isLoop;

    protected Vector3 diffAngle;
    protected Vector3 totalAngle = Vector3.zero;
    protected float totalTime = 0;

    protected void Awake()
    {
        myTran = transform;
        if (angleTime <= 0) angleTime = 1;
        diffAngle = angle / angleTime;
    }
	
	void Update ()
    {
        if (isLoop)
        {
            myTran.Rotate(diffAngle * Time.deltaTime);
        }
        else
        {
            if (angleTime <= 0) return;
            angleTime -= Time.deltaTime;
            Vector3 v = diffAngle * Time.deltaTime;
            totalAngle += v;
            if (angle.magnitude < totalAngle.magnitude) v -= totalAngle - angle;
            myTran.Rotate(v);
        }
    }

}
