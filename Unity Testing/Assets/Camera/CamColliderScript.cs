using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamColliderScript : MonoBehaviour
{
    [Header("Reffrences")]
    public CameraBehavior cameraBehavior;

    private void OnTriggerEnter(Collider other)
    {
        cameraBehavior.SetCollision(true);
    }

    private void OnTriggerExit(Collider other)
    {
        cameraBehavior.SetCollision(false);
    }
}
