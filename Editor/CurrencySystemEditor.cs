using UnityEditor;
using UnityEngine;
using Excellcube;

[CustomEditor(typeof(CurrencySystem))]
class CurrencySystemEditor : Editor {

    private void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.LabelField("Currency 데이터", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("Gold", CurrencySystem.gold.ToString());
        EditorGUILayout.LabelField("Ruby", CurrencySystem.ruby.ToString());

        Repaint();
    }
}