using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CamHitPoint
{
    public float distance;
    public CamHitPointType camHitPointType; // Mabye change this to a boolean
    public Collider collider;

    public CamHitPoint(float _distance, CamHitPointType _camHitPointType, Collider _collider)
    {
        distance = _distance;
        camHitPointType = _camHitPointType;
        collider = _collider;
    }

    public override string ToString()
    {
        if (collider == null)
            return string.Format("{0}, {1: 0.00}", camHitPointType.ToString(), distance);
        else
            return string.Format("\"{0}\": {1}, {2: 0.00}", collider.name, camHitPointType.ToString(), distance);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////

public class CamHitPointList : IEnumerable
{
    public List<CamHitPoint> camHitPoints;
    public Collider lastCamPosCollider;

    private int colliderIndex = -1;
    //////////////////////////////////////////////
    public CamHitPointList()
    {
        camHitPoints = new List<CamHitPoint>();
        lastCamPosCollider = null;
    }

    public IEnumerator GetEnumerator()
    {
        return camHitPoints.GetEnumerator();
    }

    public override string ToString()
    {
        return string.Join("--", camHitPoints);
    }
    //////////////////////////////////////////////
    public void Add(CamHitPoint value)
    {
        camHitPoints.Add(value);
    }

    public void Sort()
    {
        camHitPoints.Sort(new CamHitPointComparer());
    }

    public void FindColliderIndex()
    {
        colliderIndex = -1;

        for (int i = 0; i < camHitPoints.Count; i++)
            if (camHitPoints[i].camHitPointType == CamHitPointType.Front && camHitPoints[i].collider == lastCamPosCollider)
            {
                colliderIndex = i;
                break;
            }
    }

    public float GetCamPosDistance()
    {
        int camPosIndex = GetCamPosIndex();

        if (camPosIndex != -1)
        {
            lastCamPosCollider = camHitPoints[camPosIndex].collider;
            return camHitPoints[camPosIndex].distance;
        }

        return -1f;
    }

    int GetCamPosIndex()
    {
        int startIndex = 0;

        if (colliderIndex != -1)
            startIndex = colliderIndex;

        float betweenWalls = .4f;

        for (int i = startIndex; i < camHitPoints.Count; i++)
        {
            if (camHitPoints[i].camHitPointType == CamHitPointType.Front)
            {
                if (i < camHitPoints.Count - 1)
                {
                    if (camHitPoints[i + 1].camHitPointType == CamHitPointType.Back)
                    {
                        float deltaDistance = camHitPoints[i].distance - camHitPoints[i + 1].distance;

                        if (deltaDistance < 0)

                            throw new InvalidOperationException("Distance between two walls cannot be less than zero");

                        else if (deltaDistance >= betweenWalls)

                            return i;
                    }
                }
                else
                    return i;

            }
        }

        return -1;
    }

    public void Reset(float maxCamDis)
    {
        camHitPoints.Clear();
        camHitPoints.Add(new CamHitPoint(maxCamDis, CamHitPointType.Front, null));
        colliderIndex = -1;
    }
}

////////////////////////////////////////////////////////////////////////////////////////////

public enum CamHitPointType : byte
{
    Front,
    Back
}

////////////////////////////////////////////////////////////////////////////////////////////

public class CamHitPointComparer : IComparer<CamHitPoint>
{
    int IComparer<CamHitPoint>.Compare(CamHitPoint p1, CamHitPoint p2)
    {
        return -p1.distance.CompareTo(p2.distance);
    }
}
