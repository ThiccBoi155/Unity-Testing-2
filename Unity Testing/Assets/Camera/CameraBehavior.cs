using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraBehavior : MonoBehaviour
{
    [Header("Other debug values")]
    public bool followPlayer;

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
        
        Gizmos.color = Color.red;
        Gizmos.DrawCube(Vector3.zero, Vector3.one * .4f);
        
        

        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(0, 0, -maxCamDis), Vector3.one * .4f);

        Gizmos.color = new Color(255, 165, 0);
        Gizmos.DrawSphere(new Vector3(0, 0, -targetCamDis), .2f);

    }
}
