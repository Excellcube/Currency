using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Excellcube;

[CustomPropertyDrawer(typeof(BigNum))]
public class BigNumEditor : PropertyDrawer
{
    private Rect m_Position;
    private float m_Height = 0;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return m_Height;
    }

    // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    // {
    //     m_Position = position;
    //     float startY = m_Position.y;

    //     EditorGUI.BeginProperty(position, label, property);
        
    //     SerializedProperty valueProperty  = property.FindPropertyRelative("m_Value");

    //     double currentPrice = valueProperty.doubleValue;
    //     double newPrice = EditorGUILayout.DoubleField(label, currentPrice);

    //     if (newPrice != currentPrice)
    //     {
    //         valueProperty.doubleValue = newPrice;
    //     }

    //     property.serializedObject.ApplyModifiedProperties();

    //     EditorGUI.EndProperty();

    //     float endY = m_Position.y;
    //     m_Height = endY - startY;
    // }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        m_Position = position;
        float startY = m_Position.y;

        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty valueProperty  = property.FindPropertyRelative("m_Value");
        double currentPrice = valueProperty.doubleValue;

        // EditorGUI를 사용하면서 Rect의 위치와 크기를 명시적으로 지정
        Rect fieldPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        double newPrice = EditorGUI.DoubleField(fieldPosition, label, currentPrice);

        if (newPrice != currentPrice)
        {
            valueProperty.doubleValue = newPrice;
        }

        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();

        float endY = m_Position.y + EditorGUIUtility.singleLineHeight;
        m_Height = endY - startY;
    }
}
