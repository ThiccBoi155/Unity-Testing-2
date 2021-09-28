using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Sub Behaviors")]
    public PlayerMovementBehavior playerMovementBehavior;
    public CameraBehavior cameraBehavior;

    private Vector3 rawInputMovement;
    private Vector2 camRotationInput;

    private void Start()
    {
        SetupPlayer();
    }

    public void SetupPlayer()
    {
        playerMovementBehavior.SetupBehavior();
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            playerMovementBehavior.SetJump(true);
        }
        else
        {
            playerMovementBehavior.SetJump(false);
        }
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);
    }

    public void OnCamRotation(InputAction.CallbackContext value)
    {
        camRotationInput = value.ReadValue<Vector2>();
    }

    private void Update()
    {
        UpdatePlayerMovement();
        UpdateCamRotation();
    }

    private void UpdatePlayerMovement()
    {
        playerMovementBehavior.UpdateMovementData(rawInputMovement);
    }

    private void UpdateCamRotation()
    {
        cameraBehavior.UpdateCamRotation(camRotationInput);
    }
}
