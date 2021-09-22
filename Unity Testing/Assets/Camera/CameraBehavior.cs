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
    public bool enableMeshRenderer;
    public MeshRenderer meshRenderer;

    [Header("Refrences")]
    public Transform player;
    public Transform followCamera;

    [Header("Continuous Settings")]
    public float followSpeed;
    public float rotationSpeed;
    public float maxRotationX = 89;
    public float minRotationX = -70;

    public float cameraDistance = 8;

    private Vector2 gamepadLook;
    private Vector3 eulerAngleRotation;

    private void Start()
    {
        CameraLookAtOrigin();
    }

    void FixedUpdate()
    {
        //DebugFunctions();

        meshRenderer.enabled = enableMeshRenderer;

        if (followPlayer)
            FollowPlayer();
        RotateCamera();
        SetCameraDistance();
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
    
    void CameraLookAtOrigin()
    {
        followCamera.LookAt(transform);
    }

    void FollowPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, player.position, followSpeed * Time.fixedDeltaTime);
    }
    //////////////////////////////////////////////
    void SetCameraDistance()
    {
        followCamera.localPosition = new Vector3(0, 0, -cameraDistance);
    }
    //////////////////////////////////////////////
    public void UpdateCameraLook(Vector2 look)
    {
        gamepadLook = look;
    }
}
