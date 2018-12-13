using UnityEngine;
using System.Collections;

public class FrameAdvance : MonoBehaviour
{
    [SerializeField]
    private int framePerSecond;
    [SerializeField]
    private float secondPerFrame;

    private void OnValidate()
    {
        
    }

    private float time;

    void Update()
    {
    }

    private void SetOffset(Vector2 offset)
    {
    }

    private void OnEnable()
    {
    }
}