using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Excellcube;

public class RubyField : MonoBehaviour, ICurrencyField {
    [SerializeField]
    private Text m_ValueText;

    public void SetValue(BigNum value) {
        m_ValueText.text = value.ToShortForm();
    }
}
