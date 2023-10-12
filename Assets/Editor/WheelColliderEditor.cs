#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// [CustomEditor(typeof(WheelCollider))]
public class WheelColliderEditor : Editor
{
    private WheelCollider wheelCollider;

    public void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
#endif