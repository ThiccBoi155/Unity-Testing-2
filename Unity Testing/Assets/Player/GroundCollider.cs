using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    public PlayerMovementBehavior pmb;

    private void OnTriggerEnter(Collider other)
    {
        //pmb.GroundCollision(true);
        Debug.Log("enter");
    }

    private void OnTriggerExit(Collider other)
    {
        //pmb.GroundCollision(false);
        Debug.Log("exit");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("stay");
    }
}
