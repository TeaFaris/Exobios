using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyPropertyAttribute))]
public sealed class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        bool enabledState = GUI.enabled;

        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = enabledState;

        EditorGUI.EndProperty();
    }
}