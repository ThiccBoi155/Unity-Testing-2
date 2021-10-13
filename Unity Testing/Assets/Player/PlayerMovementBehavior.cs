using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehavior : MonoBehaviour
{
    [Header("Debug info")]
    public bool falling;
    public bool jumpButtonHeld;
    public bool onGround;

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
    //private bool jumpButtonHeld;
    //private bool onGround;
    
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

        jumpButtonHeld = b;
    }

    public void UpdateMovementData(Vector3 newMovementDirection)
    {
        movementDirection = newMovementDirection;
    }

    public void GroundCollision(bool b)
    {
        onGround = b;
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
        if (onGround)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, jumpForce, playerRb.velocity.z);
            //playerRb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
        }
    }

    void CheckJump()
    {
        if (playerRb.velocity.y < 0 /*|| falling*/)
        {
            playerRb.AddForce(new Vector3(0, -fallForce, 0), ForceMode.Acceleration);
            falling = true;
        }
        //*
        else if (playerRb.velocity.y > 0 && !jumpButtonHeld)

            playerRb.AddForce(new Vector3(0, -lowJumpForce, 0), ForceMode.Acceleration);
        //*/
        //*/
        else
            falling = false;
        //*/
    }

    /*/
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
    //*/
}
