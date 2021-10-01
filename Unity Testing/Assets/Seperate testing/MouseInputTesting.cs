using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputTesting : MonoBehaviour
{
    public CameraToPosition behavior;

    private Vector2 pointer = new Vector3();

    public void OnPointerMovement(InputAction.CallbackContext value)
    {
        behavior.SetMouseInput(value.ReadValue<Vector2>());
    }
}
