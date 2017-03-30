using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(CylinderTree))]
[CanEditMultipleObjects]
public class CylinderTreeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate"))
        {
            foreach (var t in targets)
            {
                ((CylinderTree)t).Generate();
            }
        }
    }
}
