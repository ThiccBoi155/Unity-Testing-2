using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [Header("Refrences")]
    public Transform player;
    public Transform followCamera;

    [Header("Settings")]
    public float followSpeed;
    public float rotationSpeed;

    private Vector2 gamepadLook;

    void FixedUpdate()
    {
        Debug.Log(gamepadLook);

        FollowPlayer();
        RotateCamera();
        CameraLookAtOrigin();
    }

    void RotateCamera()
    {
        transform.Rotate(new Vector3(gamepadLook.y, -gamepadLook.x, 0));
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
    public void UpdateCameraLook(Vector2 look)
    {
        gamepadLook = look;
    }
}
