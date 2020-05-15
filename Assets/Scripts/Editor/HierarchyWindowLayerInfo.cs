using UnityEditor;
using UnityEngine;

// <summary>
/// Hierarchy Window Layer Info
/// http://diegogiacomelli.com.br/unitytips-hierarchy-window-layer-info/
/// </summary>
[InitializeOnLoad]
public static class HierarchyWindowLayerInfo
{
    static readonly int IgnoreLayer = LayerMask.NameToLayer("Default");

    static readonly GUIStyle _style = new GUIStyle()
    {
        fontSize = 9,
        alignment = TextAnchor.MiddleRight
    };

    static HierarchyWindowLayerInfo()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHierarchyWindowItemOnGUI;
    }

    static void HandleHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        var gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

        if (gameObject != null && gameObject.layer != IgnoreLayer)
        {
            EditorGUI.LabelField(selectionRect, LayerMask.LayerToName(gameObject.layer), _style);
        }
    }
}