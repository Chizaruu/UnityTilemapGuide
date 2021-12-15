using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
    {
        string valueStr;

        switch (prop.propertyType)
        {
            case SerializedPropertyType.Integer:
                valueStr = prop.intValue.ToString();
                break;
            case SerializedPropertyType.Boolean:
                valueStr = prop.boolValue.ToString();
                break;
            case SerializedPropertyType.Float:
                valueStr = prop.floatValue.ToString("0.00000");
                break;
            case SerializedPropertyType.String:
                valueStr = prop.stringValue;
                break;
            case SerializedPropertyType.Enum:
                valueStr = prop.enumDisplayNames[prop.enumValueIndex];
                break;
            case SerializedPropertyType.ObjectReference:
                try {
                        valueStr = prop.objectReferenceValue.ToString ();
                    } catch (NullReferenceException) {
                        valueStr = "None (Game Object)";
                    }
                break;
            default:
                valueStr = "(not supported)";
                break;
        }

        EditorGUI.LabelField(position,label.text, valueStr);
    }
}