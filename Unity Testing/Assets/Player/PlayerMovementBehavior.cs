using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehavior : MonoBehaviour
{
    [Header("Debug info")]
    public bool falling;
    public bool framePrint;

    [Header("Component Refrences")]
    public Rigidbody playerRb;

    [Header("Movement Settings")]
    public float movementSpeed;
    public float jumpForce;
    public float fallForce;
    public float lowJumpForce;
    
    // Stored Values
    private Camera mainCamera;

    private Vector3 movementDirection;
    private bool jumpButtonHeld;
    private bool onGround;

    //////////////////////////////////////////////
    // Set values
    //////////////////////////////////////////////
    /*/
    [Header("Set velocity")]
    public Vector3 velocityToSet;
    public bool setVelocityNow;

    [Header("Set angular velocity")]
    public Vector3 angularVelocityToSet;
    public bool setAngularVelocityNow;

    [Header("Set force")]
    public Vector3 forceToSet;
    public ForceMode forceModeToSet;
    public bool setForceNow;
    public bool setContinuously;

    private void Update()
    {
        if (setVelocityNow)
        {
            playerRidgidbody.velocity = velocityToSet;
            setVelocityNow = false;
        }
        if (setAngularVelocityNow)
        {
            playerRidgidbody.angularVelocity = angularVelocityToSet;
            setAngularVelocityNow = false;
        }
        if (setForceNow)
        {
            playerRidgidbody.AddForce(forceToSet, forceModeToSet);
            setForceNow = false;
        }
    }
    //*/
    //////////////////////////////////////////////

    public void SetupBehavior()
    {
        SetGameplayCamera();
    }

    void SetGameplayCamera()
    {
        mainCamera = Camera.main;
    }
    //////////////////////////////////////////////
    public void SetJump(bool b)
    {
        if (b)
            Jump();

        jumpButtonHeld = true;
    }

    public void UpdateMovementData(Vector3 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }
    //////////////////////////////////////////////
    private void Update()
    {
        if (framePrint)
            Debug.Log("Update");
    }
    private void LateUpdate()
    {
        if (framePrint)
            Debug.Log("Late Update");
    }

    private void FixedUpdate()
    {
        if (framePrint)
            Debug.Log("Fixed Update");
        CheckJump();
        MoveThePlayer();
    }

    void MoveThePlayer()
    {
        //Vector3 movement = CameraDirection(movementDirection) * movementSpeed * Time.deltaTime;
        Vector3 movement = CameraDirection(movementDirection) * movementSpeed * Time.fixedDeltaTime;
        playerRb.MovePosition(transform.position + movement);
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
    //////////////////////////////////////////////
    void Jump()
    {
        playerRb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
    }

    void CheckJump()
    {
        if (playerRb.velocity.y < 0 /*|| falling*/)
        {
            playerRb.AddForce(new Vector3(0, -fallForce, 0), ForceMode.Acceleration);
            falling = true;
        }
        /*
        else if (playerRb.velocity.y > 0 && !jumpButtonHeld)

            playerRb.AddForce(new Vector3(0, -lowJumpForce, 0), ForceMode.Acceleration);
        //*/
        //*/
        else
            falling = false;
        //*/
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}
