using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FXV.Internal.Shield
{
    public class fxvSortingLayerAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(fxvSortingLayerAttribute))]
        public class SortingLayerDrawer : PropertyDrawer
        {
            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                string[] sortingLayerNames = new string[SortingLayer.layers.Length];
                for (int a = 0; a < SortingLayer.layers.Length; a++)
                    sortingLayerNames[a] = SortingLayer.layers[a].name;
                if (property.propertyType != SerializedPropertyType.String)
                {
                    EditorGUI.HelpBox(position, property.name + "{0} is not an string but has [SortingLayer].", MessageType.Error);
                }
                else if (sortingLayerNames.Length == 0)
                {
                    EditorGUI.HelpBox(position, "There is no Sorting Layers.", MessageType.Error);
                }
                else if (sortingLayerNames != null)
                {
                    EditorGUI.BeginProperty(position, label, property);

                    // Look up the layer name using the current layer ID
                    string oldName = property.stringValue;

                    // Use the name to look up our array index into the names list
                    int oldLayerIndex = -1;
                    for (int a = 0; a < sortingLayerNames.Length; a++)
                        if (sortingLayerNames[a].Equals(oldName)) oldLayerIndex = a;

                    // Show the popup for the names
                    int newLayerIndex = EditorGUI.Popup(position, label.text, oldLayerIndex, sortingLayerNames);

                    // If the index changes, look up the ID for the new index to store as the new ID
                    if (newLayerIndex != oldLayerIndex)
                    {
                        property.stringValue = sortingLayerNames[newLayerIndex];
                    }

                    EditorGUI.EndProperty();
                }
            }
        }
#endif
    }
}