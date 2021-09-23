using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Continuous Settings")]
    public float followSpeed;
    public float rotationSpeed;
    public float maxRotationX = 89;
    public float minRotationX = -70;
    public float maxCamDis = 8;

    private float targetCamDis;
    private float currentCamDis;

    private Vector2 gamepadLook;
    private Vector3 eulerAngleRotation;

    //////////////////////////////////////////////
    void FixedUpdate()
    {
        // Debug
        foreach (RefrenceSetActive meshRef in debugMeshRefrences)
            meshRef.SetActive();

        // Max
        debugMeshRefrences[1].obj.transform.localPosition = new Vector3(0, 0, -maxCamDis);
        // Target
        debugMeshRefrences[2].obj.transform.localPosition = new Vector3(0, 0, -targetCamDis);

        // Other
        if (followPlayer)
            FollowPlayer();

        RotateCamera();

        // Camera Distance
        CalculateTargetCamDis();
        CalculateCurrentCamDis();
        SetCamDis();
    }

    void RotateCamera()
    {
        // Set eulerAngleRotation
        eulerAngleRotation.x += -gamepadLook.y * rotationSpeed * Time.fixedDeltaTime;
        eulerAngleRotation.y += gamepadLook.x * rotationSpeed * Time.fixedDeltaTime;

        if (eulerAngleRotation.x < minRotationX)
            eulerAngleRotation.x = minRotationX;

        if (maxRotationX < eulerAngleRotation.x)
            eulerAngleRotation.x = maxRotationX;

        // Reset rotation
        transform.rotation = new Quaternion();

        // Set rotation to eulerAngleRotation
        transform.Rotate(eulerAngleRotation, Space.World);
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, followSpeed * Time.fixedDeltaTime);
    }
    //////////////////////////////////////////////
    // Camera distance / CamDis
    //////////////////////////////////////////////
    void CalculateTargetCamDis()
    {
        targetCamDis = 6;
        /*
        RaycastHit hit;
        bool b = Physics.Raycast(transform.position, -transform.forward, out hit, maxCamDis);

        Debug.Log(b);

        if (b)
        {
            targetCamDis = hit.distance;
            Debug.Log(hit.transform.name);
        }
        else
            targetCamDis = maxCamDis;
        //*/
    }

    void CalculateCurrentCamDis()
    {
        currentCamDis = 4;
        //currentCamDis = targetCamDis;
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
    public void UpdateCameraLook(Vector2 look)
    {
        gamepadLook = look;
    }
}
