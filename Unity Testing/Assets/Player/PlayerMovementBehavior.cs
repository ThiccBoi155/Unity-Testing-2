using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehavior : MonoBehaviour
{
    [Header("Component Refrences")]
    public Rigidbody playerRidgidbody;

    [Header("Movement Settings")]
    public float movementSpeed = 3f;
    
    // Stored Values
    private Camera mainCamera;
    private Vector3 movementDirection;

    //////////////////////////////////////////////
    // Set values
    //////////////////////////////////////////////
    //*/
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
        //playerRidgidbody.AddForce(new Vector3(0, 9.81f, 0));
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

        if (setContinuously)
            playerRidgidbody.AddForce(forceToSet, forceModeToSet);

        //playerRidgidbody.AddForce(new Vector3(0, 9.81f * playerRidgidbody.mass, 0), ForceMode.Force);

        Vector2 v = playerRidgidbody.velocity;
        Debug.Log(v);
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
