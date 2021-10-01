using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraToPosition : MonoBehaviour
{
    Vector2 mouseInput = new Vector2();

    public Transform screenToWorldPointObject;
    public Vector3 position;

    public Transform fillDisplayObject;
    public float distance;

    private void Update()
    {
        SetScreenToWorldPointObjectPosition();

        SetFillDisplayObjectValues();
    }

    void SetScreenToWorldPointObjectPosition()
    {
        Vector3 mouseWorldPosition2D = Camera.main.ScreenToWorldPoint(position);
        Debug.Log(mouseWorldPosition2D);
        screenToWorldPointObject.position = mouseWorldPosition2D;
    }

    void SetFillDisplayObjectValues()
    {
        fillDisplayObject.position = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f, distance));
        fillDisplayObject.rotation = Camera.main.transform.rotation;

        Vector3 buttomLeftCorner = Camera.main.ViewportToWorldPoint(new Vector3(0f, 0f, distance));
        Vector3 upperRightCorner = Camera.main.ViewportToWorldPoint(new Vector3(1f, 1f, distance));

        fillDisplayObject.localScale = upperRightCorner - buttomLeftCorner;
    }

    /////////////////////////////////////////////////////////////////////////////////////
    void SetPosition()
    {
        //Debug.Log(mouseInput);
        Vector3 mouseWorldPosition2D = Camera.main.ScreenToWorldPoint(mouseInput);
        Debug.Log(mouseWorldPosition2D);
    }

    public void SetMouseInput(Vector2 value)
    {
        mouseInput = value;
        
        //SetPosition();
    }
}
