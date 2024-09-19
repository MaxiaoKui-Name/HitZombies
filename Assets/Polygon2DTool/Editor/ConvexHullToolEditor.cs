using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _01_ConvexHull
{
    [CustomEditor(typeof(ConvexHullTool))]
    public class ConvexHullToolEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ConvexHullTool myScript = (ConvexHullTool)target;
            if (GUILayout.Button("Éú³ÉÅö×²Ìå"))
                myScript.Computer();
        }
    }
}
