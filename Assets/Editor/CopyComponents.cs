using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class CopyComponents : Editor
{

    static Component[] compoentArr;

    [MenuItem("Component Editor/Copy Components")]
    static void DoCopyComponent()
    {
        compoentArr = Selection.activeGameObject.GetComponents<Component>();
    }

    [MenuItem("Component Editor/Paste Components")]
    static void DoPasteComponent()
    {
        if (compoentArr == null)
        {
            return;
        }

        GameObject targetObject = Selection.activeGameObject;
        if (targetObject == null)
        {
            return;
        }

        for (int i = 0; i < compoentArr.Length; i++)
        {
            Component newComponent = compoentArr[i];
            if (newComponent == null)
            {
                continue;
            }
            UnityEditorInternal.ComponentUtility.CopyComponent(newComponent);
            Component oldComponent = targetObject.GetComponent(newComponent.GetType());
            if (oldComponent != null)
            {
                if (UnityEditorInternal.ComponentUtility.PasteComponentValues(oldComponent))
                {
                    Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Success");
                }
                else
                {
                    Debug.Log("Paste Values " + newComponent.GetType().ToString() + " Failed");
                }
            }
            else
            {
                if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(targetObject))
                {
                    Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Success");
                }
                else
                {
                    Debug.Log("Paste New Values " + newComponent.GetType().ToString() + " Failed");
                }
            }
        }
    }
}