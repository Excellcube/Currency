using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Excellcube;

[CustomEditor(typeof(PurchaseButton))]
public class PurchaseButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var iconProp         = serializedObject.FindProperty("m_Icon");
        var priceTextProp    = serializedObject.FindProperty("m_PriceText");
        var disableTextProp  = serializedObject.FindProperty("m_DisableText");
        var buttonProp       = serializedObject.FindProperty("m_Button");

        var currencyTypeProp = serializedObject.FindProperty("m_CurrencyType");
        var priceProp        = serializedObject.FindProperty("m_Price");

        var onPurchaseProp   = serializedObject.FindProperty("m_OnPurchase");
        var onFailureProp    = serializedObject.FindProperty("m_OnFailure");


        EditorGUILayout.PropertyField(iconProp);
        EditorGUILayout.PropertyField(priceTextProp);
        EditorGUILayout.PropertyField(disableTextProp);
        EditorGUILayout.PropertyField(buttonProp);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(currencyTypeProp);

        // m_Price 필드의 값이 변경되면 내부의 Text를 업데이트.
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(priceProp);
        if(EditorGUI.EndChangeCheck()) 
        {
            SerializedObject priceTextSO = new SerializedObject(priceTextProp.objectReferenceValue);
            SerializedProperty textProp  = priceTextSO.FindProperty("m_Text");

            if(textProp != null)
            {
                var priceValueProp = priceProp.FindPropertyRelative("m_Value");
                BigNum price = priceValueProp.doubleValue;
                textProp.stringValue = price.ToShortForm();
                priceTextSO.ApplyModifiedProperties();
            }
        }

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(onPurchaseProp);
        EditorGUILayout.PropertyField(onFailureProp);


        // property에서 발생한 변경사항 반영.
        if(GUI.changed)
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
