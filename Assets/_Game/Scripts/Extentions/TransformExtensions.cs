using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtentions
{
    public static void SlowLookAt(this Transform t, Vector3 target, float rotateSpeed)
    {
        Vector3 lookPos = target - t.position;

        Quaternion lookRot = Quaternion.LookRotation(lookPos);

        t.rotation = Quaternion.Slerp(t.rotation, lookRot, Time.deltaTime * rotateSpeed);
    }

    public static void SlowLookAt(this Transform t, Transform target, float rotateSpeed) => SlowLookAt(t, target.position, rotateSpeed);

    public static Vector3 GetPointAround(this Transform t, Vector3 direction, float angle, float distance, bool useLocalForward = true)
    {
        return t.position + Quaternion.Euler(direction.normalized * angle) * (useLocalForward ? t.forward : Vector3.forward) * distance;
    }

    #region Curves and Middle Points

    public static Vector3 GetMiddlePosition(this Vector3 t, Transform target, float weight = 0.5f)
    {
        return t.GetMiddlePosition(target.position, weight);
    }

    public static Vector3 GetMiddlePosition(this Vector3 t, Vector3 target, float weight = 0.5f)
    {
        return t + (target - t) * weight;
    }

    public static Vector3 GetMiddlePosition(this Transform t, Transform target, float weight = 0.5f)
    {
        return t.GetMiddlePosition(target.position, weight);
    }

    public static Vector3 GetMiddlePosition(this Transform t, Vector3 target, float weight = 0.5f)
    {
        return t.position + (target - t.position) * weight;
    }

    public static Vector3 GetMiddlePointOfArray(this Transform[] t)
    {
        Vector3 v = Vector3.zero;

        for (int i = 0; i < t.Length; i++)
        {
            v += t[i].position;
        }

        return v / t.Length;
    }

    public static Vector3 GetQuadraticMiddlePoint(this Transform t, Transform pA, Transform pEnd, float weight = .5f) => GetQuadraticMiddlePoint(t, pA.position, pEnd.position, weight);

    public static Vector3 GetQuadraticMiddlePoint(this Transform t, Vector3 pointA, Vector3 endPoint, float weight = .5f)
    {
        Vector3 p0 = GetMiddlePosition(t, pointA, weight);
        Vector3 p1 = GetMiddlePosition(pointA, endPoint, weight);

        return GetMiddlePosition(p0, p1, weight);
    }

    public static Vector3 GetQuadraticMiddlePoint(this Vector3 t, Vector3 pointA, Vector3 endPoint, float weight = .5f)
    {
        Vector3 p0 = GetMiddlePosition(t, pointA, weight);
        Vector3 p1 = GetMiddlePosition(pointA, endPoint, weight);

        return GetMiddlePosition(p0, p1, weight);
    }

    public static Vector3 GetCubicMiddlePoint(this Vector3 v, Vector3 pointA, Vector3 pointB, Vector3 pointC, float weight = 0.5f)
    {
        Vector3 p0 = v.GetQuadraticMiddlePoint(pointA, pointB, weight);
        Vector3 p1 = pointA.GetQuadraticMiddlePoint(pointB, pointC, weight);

        return GetMiddlePosition(p0, p1, weight);
    }

    public static Vector3[] GetQuadraticPath(this Vector3 v, Vector3 pointA, Vector3 endPoint, int resolution)
    {
        Vector3[] _v = new Vector3[resolution];
        float weigth = 0;
        for (int i = 0; i < resolution; i++)
        {
            weigth = ((float)i + 1f) / (float)resolution;
            _v[i] = v.GetQuadraticMiddlePoint(pointA, endPoint, weigth);
        }

        return _v;
    }

    public static Vector3[] GetQuadraticPath(this Vector3 v, Transform pointA, Transform endPoint, int resolution) => GetQuadraticPath(v, pointA.position, endPoint.position, resolution);

    public static Vector3[] GetQuadraticPath(this Transform t, Vector3 pointA, Vector3 endPoint, int resolution)
    {
        Vector3[] _v = new Vector3[resolution];
        float weigth = 0;
        for (int i = 0; i < resolution; i++)
        {
            weigth = ((float)i + 1f) / (float)resolution;
            _v[i] = t.GetQuadraticMiddlePoint(pointA, endPoint, weigth);
        }

        return _v;
    }

    public static Vector3[] GetQuadraticPath(this Transform t, Transform pointA, Transform endPoint, int resolution) => GetQuadraticPath(t, pointA.position, endPoint.position, resolution);

    #endregion
}
