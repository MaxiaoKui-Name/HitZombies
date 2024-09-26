using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshRenderer))]
public class MeshRenderEditor : Editor
{
    MeshRenderer meshRenderer;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        meshRenderer = target as MeshRenderer;

        string[] layerNames = new string[SortingLayer.layers.Length];
        for (int i = 0; i < layerNames.Length; i++)
        {
            layerNames[i] = SortingLayer.layers[i].name;
        }

        int layerValue = SortingLayer.GetLayerValueFromID(meshRenderer.sortingLayerID);
        layerValue = EditorGUILayout.Popup("Sorting Layer", layerValue, layerNames);

        if (layerValue <= -1 || layerValue >= SortingLayer.layers.Length)
            layerValue = 0;
        SortingLayer layer = SortingLayer.layers[layerValue];
        meshRenderer.sortingLayerName = layer.name;
        meshRenderer.sortingLayerID = layer.id;
        meshRenderer.sortingOrder = EditorGUILayout.IntField("Order In Value", meshRenderer.sortingOrder);
    }
}