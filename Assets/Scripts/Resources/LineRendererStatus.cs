using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererStatus
{
    private List<Vector2> points;
    public int pointCount;
    public Vector2 startPoint;
    public Vector2 endPoint;
    private float _distance = -1;
    public float distance
    {
        get { return _distance >= 0 ? _distance : _distance = (startPoint - endPoint).magnitude; }
    }
    private float _length = -1;
    public float length
    {
        get {
            if (_length < 0)
            {
                for (int i = 1; i < pointCount - 1; i++)
                {
                    _length += (points[i] - points[i - 1]).magnitude;
                }
            }
            return _length;
        }
    }
    private float _rate = -1;
    public float rate
    {
        get {
            if (_rate < 0)
            {
                if (length == 0)
                {
                    _rate = 0;
                } else
                {
                    _rate = Mathf.Round(distance / length * 100);
                }
            }
            return _rate;
        }
    }

    public LineRendererStatus(List<Vector2> list)
    {
        points = list;
        pointCount = points.Count;
        startPoint = pointCount > 0 ? points[0] : Vector2.zero;
        endPoint = pointCount > 0 ? points[pointCount - 1] : Vector2.zero;
    }

    public List<Vector2> GetEvenlySpacedPoints(int maxCount, float perLength)
    {
        if (perLength > 0)
        {
            return GetEvenlySpacedPoints(perLength);
        }
        return GetEvenlySpacedPoints(maxCount);
    }
    public List<Vector2> GetEvenlySpacedPoints(int maxCount)
    {
        if (maxCount >= pointCount) return points;
        float perLength = length / maxCount;
        return GetEvenlySpacedPoints(perLength);
    }
    public List<Vector2> GetEvenlySpacedPoints(float perLength)
    {
        List<Vector2> list = new List<Vector2>();
        float total = 0;
        list.Add(startPoint);
        for (int i = 1; i < pointCount; i++)
        {
            total += (points[i - 1] - points[i]).magnitude;
            if (perLength * list.Count <= total)
            {
                list.Add(points[i]);
            }
        }
        return list;
    }
}
