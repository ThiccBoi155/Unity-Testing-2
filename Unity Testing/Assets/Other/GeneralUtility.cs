using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeneralUtility
{
    public static void ClearConsole()
    {
        System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.LogEntries").GetMethod("Clear").Invoke(new object(), null);
    }
}
