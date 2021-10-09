using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraBehavior : MonoBehaviour
{
    [Header("Other debug values")]
    public bool followPlayer;
    public bool hideCamOriginGizmo;
    public bool hideTargetCamDisGizmo;
    public bool hideMaxCamDisGizmo;

    [Header("Refrences")]
    public Transform player;
    public Transform followCamera;

    [Header("Continuous Settings")]
    public float originFollowSpeed;

    public float rotationSpeed;
    public float gamepadSmoothing;
    public float maxRotationX = 89;
    public float minRotationX = -70;

    public float maxCamDis = 8;
    public float camDisFollowSpeed;
    public LayerMask camCollisionLayerMask;

    [Header("Private fields")]
    [SerializeField]
    private float targetCamDis;
    [SerializeField]
    private float currentCamDis;

    [SerializeField]
    private bool camColliding = false;
    [SerializeField]
    private bool wallDetected = false;

    private Vector2 rawGamepadInput;
    private Vector2 smoothGamepadInput;
    private Vector3 eulerAngleRotation;

    private RaycastHit[] originToCameraHits = new RaycastHit[0];
    private RaycastHit[] cameraToOriginHits = new RaycastHit[0];

    private List<CamHitPoint> camHitPoints = new List<CamHitPoint>();
    private Collider currentTargetCamDisCollider = null;

    //////////////////////////////////////////////
    void FixedUpdate()
    {
        // Other
        if (followPlayer)
            FollowPlayer();

        CalculateSmoothGamepadInput();
        RotateCamera();

        // Camera Distance
        CalculateTargetCamDis();
        CalculateCurrentCamDis();
        SetCamDis();

        SetCamHitPoints();
        Temp();
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, originFollowSpeed * Time.fixedDeltaTime);
    }

    void CalculateSmoothGamepadInput()
    {
        smoothGamepadInput = Vector2.Lerp(smoothGamepadInput, rawGamepadInput, gamepadSmoothing * Time.fixedDeltaTime);
    }

    void RotateCamera()
    {
        // Set eulerAngleRotation
        eulerAngleRotation.x += -smoothGamepadInput.y * rotationSpeed * Time.fixedDeltaTime;
        eulerAngleRotation.y += smoothGamepadInput.x * rotationSpeed * Time.fixedDeltaTime;

        if (eulerAngleRotation.x < minRotationX)
            eulerAngleRotation.x = minRotationX;

        if (maxRotationX < eulerAngleRotation.x)
            eulerAngleRotation.x = maxRotationX;

        // Reset rotation
        transform.rotation = new Quaternion();

        // Set rotation to eulerAngleRotation
        transform.Rotate(eulerAngleRotation, Space.World);
    }
    //////////////////////////////////////////////
    // Camera distance / CamDis
    //////////////////////////////////////////////
    void CalculateTargetCamDis()
    {
        // Get ray distance
        float rayDistance;

        if (camColliding || wallDetected)
            rayDistance = GetRayDistance();
        else
            rayDistance = -1;

        // Set target cam distance
        if (rayDistance != -1)
        {
            wallDetected = true;
            targetCamDis = rayDistance;
            if (currentCamDis > targetCamDis)
                currentCamDis = targetCamDis;

            ///if (rayDistance < 1)
            //    Debug.Log("why?");
        }
        else
        {
            wallDetected = false;
            targetCamDis = maxCamDis;
        }
    }

    float GetRayDistance()
    {
        RaycastHit hit;
        bool hitBool = Physics.Raycast(transform.position, -transform.forward, out hit, maxCamDis, camCollisionLayerMask);

        ///Debug.Log(hitBool);

        if (hitBool)
        {
            ///if (hit.distance < 1)
            //    Debug.Log(hit.transform.name);
            return hit.distance;
        }
        else
            return -1;
    }

    void CalculateCurrentCamDis()
    {
        currentCamDis = Mathf.Lerp(currentCamDis, targetCamDis, camDisFollowSpeed * Time.fixedDeltaTime);
    }

    void SetCamDis()
    {
        followCamera.localPosition = new Vector3(0, 0, -currentCamDis);
    }

    void SetCamHitPoints()
    {
        originToCameraHits = Physics.RaycastAll(transform.position, -transform.forward, maxCamDis, camCollisionLayerMask);

        Vector3 maxCamDisPosition = transform.position + -transform.forward * maxCamDis;

        cameraToOriginHits = Physics.RaycastAll(maxCamDisPosition, transform.forward, maxCamDis, camCollisionLayerMask);

        camHitPoints.Clear();
        camHitPoints.Add(new CamHitPoint(maxCamDis, CamHitPointType.Front, null));

        foreach (RaycastHit hit in originToCameraHits)
        {
            camHitPoints.Add(new CamHitPoint(hit.distance, CamHitPointType.Front, hit.collider));
        }

        foreach (RaycastHit hit in cameraToOriginHits)
        {
            camHitPoints.Add(new CamHitPoint(maxCamDis - hit.distance, CamHitPointType.Back, hit.collider));
        }

        GeneralUtility.ClearConsole();

        Debug.Log(string.Join("--", camHitPoints));

        camHitPoints.Sort(new CamHitPointComparer());

        Debug.Log(string.Join("--", camHitPoints));
    }

    void Temp()
    {
        int colliderIndex = GetColliderIndex();

        int newTargetIndex;
        //*/
        if (colliderIndex != -1)
            newTargetIndex = GetNewTargetIndex(colliderIndex);
        else
            //*/
            newTargetIndex = GetNewTargetIndex();

        Debug.Log(newTargetIndex);
        
        targetCamDis = camHitPoints[newTargetIndex].distance;
        currentTargetCamDisCollider = camHitPoints[newTargetIndex].collider;
    }

    int GetNewTargetIndex(int startIndex = 0)
    {
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

    int GetColliderIndex()
    {
        for (int i = 0; i < camHitPoints.Count; i++)
            if (camHitPoints[i].collider == currentTargetCamDisCollider)
                return i;

        return -1;
    }
    //////////////////////////////////////////////
    // Start functions
    //////////////////////////////////////////////
    private void Start()
    {
        CameraLookAtOrigin();

        targetCamDis = maxCamDis;
    }

    void CameraLookAtOrigin()
    {
        followCamera.LookAt(transform);
    }
    //////////////////////////////////////////////
    // Outside functions
    //////////////////////////////////////////////
    public void UpdateCamRotation(Vector2 camRotation)
    {
        rawGamepadInput = camRotation;
    }

    public void SetCollision(bool b)
    {
        camColliding = b;
    }
    //////////////////////////////////////////////
    // Gizmos
    //////////////////////////////////////////////
    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        
        if (!hideCamOriginGizmo)
        {
            // Camera origin representation (cube)
            Gizmos.color = Color.red;
            Gizmos.DrawCube(Vector3.zero, Vector3.one * .4f);
        }

        if (!hideMaxCamDisGizmo)
        {
            // Max cam dis (cube)
            Gizmos.color = Color.green;
            Gizmos.DrawCube(new Vector3(0, 0, -maxCamDis), Vector3.one * .4f);
        }

        if (!hideTargetCamDisGizmo)
        {
            // Target cam dis (sphere)
            Gizmos.color = new Color(255, 165, 0);
            Gizmos.DrawSphere(new Vector3(0, 0, -targetCamDis), .2f);
        }

        // Current cam dis is represented with the camera gizmo

        foreach (CamHitPoint camHitPoint in camHitPoints)
        {
            if (camHitPoint.camHitPointType == CamHitPointType.Front)
            {
                Gizmos.color = Color.cyan;
            }
            else if (camHitPoint.camHitPointType == CamHitPointType.Back)
            {
                Gizmos.color = Color.blue;
            }

            Gizmos.DrawSphere(new Vector3(0, 0, -camHitPoint.distance), .15f);
        }
    }
}

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
        //return string.Format("{0}", distance);
        //return string.Format("Distance: {0}, type: {1}, collider name: {2}", distance, camHitPointType.ToString(), collider.name);
        if (collider == null)
            return string.Format("{0}, {1: 0.00}", camHitPointType.ToString(), distance);
        else
            return string.Format("\"{0}\": {1}, {2: 0.00}", collider.name, camHitPointType.ToString(), distance);
    }
}

public class CamHitPointList
{
    public List<CamHitPoint> camHitPoints;
    private Collider lastCamPosCollider = null;

    public override string ToString()
    {
        return string.Join("--", camHitPoints);
    }
}

public enum CamHitPointType : byte
{
    Front,
    Back
}

public class CamHitPointComparer : IComparer<CamHitPoint>
{
    int IComparer<CamHitPoint>.Compare(CamHitPoint p1, CamHitPoint p2)
    {
        return -p1.distance.CompareTo(p2.distance);
    }
}
