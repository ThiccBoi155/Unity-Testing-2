using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //[Header("Input Settings")]
    //public PlayerInput playerInput;

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

        Debug.Log(inputMovement);
    }
}
