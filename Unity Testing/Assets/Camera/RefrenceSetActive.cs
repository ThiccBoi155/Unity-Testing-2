using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RefrenceSetActive
{
    public GameObject obj;
    public bool enable;

    public void SetActive()
    {
        obj.SetActive(enable);
    }
}
