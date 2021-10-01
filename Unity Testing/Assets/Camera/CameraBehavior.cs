using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraBehavior : MonoBehaviour
{
    #region Debug
    /*/
    [Header("Debug values")]
    public bool rotateX;
    public bool rotateY;
    public Vector3 customRotation;
    public bool applyCustomRotation;
    public bool resetRotation;

    void DebugFunctions()
    {
        if (applyCustomRotation)
        {
            transform.Rotate(customRotation, Space.World);
            applyCustomRotation = false;
        }

        if (resetRotation)
        {
            transform.rotation = new Quaternion();
            resetRotation = false;
        }
    }
    //*/
    #endregion

    [Header("Other debug values")]
    public bool followPlayer;
    public RefrenceSetActive[] debugMeshRefrences;

    [Header("Refrences")]
    public Transform player;
    public Transform followCamera;
    public Transform maxCamDisCollider;

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

    //////////////////////////////////////////////
    void FixedUpdate()
    {
        // Debug
        foreach (RefrenceSetActive meshRef in debugMeshRefrences)
            meshRef.SetActive();

        // Max
        //debugMeshRefrences[1].obj.transform.localPosition = new Vector3(0, 0, -maxCamDis);
        // Target
        debugMeshRefrences[2].obj.transform.localPosition = new Vector3(0, 0, -targetCamDis);

        // Other
        if (followPlayer)
            FollowPlayer();

        CalculateSmoothGamepadInput();
        RotateCamera();

        //float f = rawGamepadInput.magnitude * rotationSpeed * Time.fixedDeltaTime;
        //Debug.Log($"{rawGamepadInput}, {f}");

        // Camera Distance
        SetMaxCamDisCollider();
        CalculateTargetCamDis();
        CalculateCurrentCamDis();
        SetCamDis();
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
    void SetMaxCamDisCollider()
    {
        maxCamDisCollider.localPosition = new Vector3(0, 0, -maxCamDis);
    }

    void CheckMaxCamDisCollision()
    {
        RaycastHit hit;
        bool b = Physics.Raycast(maxCamDisCollider.position, -transform.forward, out hit, 1);

        if (b)
        {
            Debug.Log($"Distance: {hit.distance}, collider object name: {hit.transform.name}");
        }
    }

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
    public void UpdateCamRotation(Vector2 camRotation)
    {
        rawGamepadInput = camRotation;
    }

    public void SetCollision(bool b)
    {
        camColliding = b;
    }
}
