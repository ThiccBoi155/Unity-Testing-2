using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Sub Behaviors")]
    public PlayerMovementBehavior playerMovementBehavior;

    private Vector3 rawInputMovement;

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
            Debug.Log("Jump");
        }
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInputMovement = new Vector3(inputMovement.x, 0, inputMovement.y);

        Debug.Log(inputMovement);
        
    }

    private void Update()
    {
        UpdatePlayerMovement();
    }

    private void UpdatePlayerMovement()
    {
        playerMovementBehavior.UpdateMovementData(rawInputMovement);
    }
}
