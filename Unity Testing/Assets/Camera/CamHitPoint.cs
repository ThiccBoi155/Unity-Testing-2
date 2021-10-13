using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CamHitPoint
{
    public float distance;
    public CamHitPointType type; // Mabye change this to a boolean
    public Collider collider;

    public CamHitPoint(float _distance, CamHitPointType _type, Collider _collider)
    {
        distance = _distance;
        type = _type;
        collider = _collider;
    }

    public override string ToString()
    {
        if (collider == null)
            return string.Format("{0}, {1: 0.00}", type.ToString(), distance);
        else
            return string.Format("\"{0}\": {1}, {2: 0.00}", collider.name, type.ToString(), distance);
    }
}

////////////////////////////////////////////////////////////////////////////////////////////

public class CamHitPointList : IEnumerable
{
    public List<CamHitPoint> camHitPoints;
    public Collider lastCamPosCollider;

    private int colliderIndex;
    private int currentCamDisIndex;
    //////////////////////////////////////////////
    public CamHitPointList()
    {
        camHitPoints = new List<CamHitPoint>();
        lastCamPosCollider = null;

        ResetReferenceIndexes();
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
            if (camHitPoints[i].type == CamHitPointType.Front && camHitPoints[i].collider == lastCamPosCollider)
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
            if (camHitPoints[i].type == CamHitPointType.Front || camHitPoints[i].type == CamHitPointType.MaxCamDis)
            {
                if (i < camHitPoints.Count - 1)
                {
                    if (camHitPoints[i + 1].type == CamHitPointType.Back)
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
        camHitPoints.Add(new CamHitPoint(maxCamDis, CamHitPointType.MaxCamDis, null));

        ResetReferenceIndexes();
    }

    void ResetReferenceIndexes()
    {
        colliderIndex = -1;
        currentCamDisIndex = -1;
    }

    //////////////////////////////////////////////
    
    public float ChangeCurrentCamDis(float currentCamDis)
    {
        InsertCurrentCamDis(currentCamDis);

        Debug.Log(this);

        if (IndexWithinWall(currentCamDisIndex))
        {
            float newCamDis = camHitPoints[currentCamDisIndex + 1].distance;

            camHitPoints.RemoveAt(currentCamDisIndex);
            InsertCurrentCamDis(newCamDis);

            return newCamDis;
        }
            

        return currentCamDis;
    }

    void InsertCurrentCamDis(float currentCamDis)
    {
        for (int i = 0; i < camHitPoints.Count; i++)
        
            if (currentCamDis > camHitPoints[i].distance)
            {
                camHitPoints.Insert(i, new CamHitPoint(currentCamDis, CamHitPointType.CurrentCamDis, null));
                ResetReferenceIndexes();
                currentCamDisIndex = i;
                
                break;
            }

        if (currentCamDisIndex == -1)
        {
            camHitPoints.Add(new CamHitPoint(currentCamDis, CamHitPointType.CurrentCamDis, null));
            ResetReferenceIndexes();
            currentCamDisIndex = camHitPoints.Count - 1;
        }
    }

    bool IndexWithinWall(int index)
    {
        Debug.Log($"index: {index}, camHitPoints.Count: {camHitPoints.Count}");

        if (0 < index && (camHitPoints[index - 1].type == CamHitPointType.Back || camHitPoints[index - 1].type == CamHitPointType.PastMaxBack))
            return true;

        if (index < camHitPoints.Count - 1 && (camHitPoints[index + 1].type == CamHitPointType.Front || camHitPoints[index + 1].type == CamHitPointType.PastMaxFront))
            return true;

        return false;
    }
}

////////////////////////////////////////////////////////////////////////////////////////////

public enum CamHitPointType : byte
{
    Front,
    Back,
    MaxCamDis,
    CurrentCamDis,
    PastMaxFront,
    PastMaxBack
}

////////////////////////////////////////////////////////////////////////////////////////////

public class CamHitPointComparer : IComparer<CamHitPoint>
{
    int IComparer<CamHitPoint>.Compare(CamHitPoint p1, CamHitPoint p2)
    {
        return -p1.distance.CompareTo(p2.distance);
    }
}
