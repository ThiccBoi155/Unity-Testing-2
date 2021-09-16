using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehavior : MonoBehaviour
{
    [Header("Component Refrences")]
    public Rigidbody playerRidgidbody;

    [Header("Movement Settings")]
    public float movementSpeed = 3f;

    [Header("Set velocity")]
    public Vector3 velocityToSet;
    public bool setVelocityNow;

    // Stored Values
    private Camera mainCamera;
    private Vector3 movementDirection;

    //////////////////////////////////////////////
    // Set values
    //////////////////////////////////////////////

    private void Update()
    {
        if (setVelocityNow)
        {
            playerRidgidbody.velocity = velocityToSet;
            setVelocityNow = false;
        }
    }

    //////////////////////////////////////////////

    public void SetupBehavior()
    {
        SetGameplayCamera();
    }

    void SetGameplayCamera()
    {
        mainCamera = Camera.main;
    }

    public void UpdateMovementData(Vector3 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }

    private void FixedUpdate()
    {
        MoveThePlayer();

        Debug.Log(playerRidgidbody.velocity);
    }

    void MoveThePlayer()
    {
        Vector3 movement = CameraDirection(movementDirection) * movementSpeed * Time.deltaTime;
        playerRidgidbody.MovePosition(transform.position + movement);
    }

    Vector3 CameraDirection(Vector3 movementDirection)
    {
        var cameraForward = mainCamera.transform.forward;
        var cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward = Vector3.Normalize(cameraForward);
        cameraRight = Vector3.Normalize(cameraRight);

        return cameraForward * movementDirection.z + cameraRight * movementDirection.x;
    }

    void Jump()
    {
        //playerRidgidbody.velocity
    }
}
