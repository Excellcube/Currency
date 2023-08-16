using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Excellcube;

public class RubyField : MonoBehaviour, ICurrencyField {
    [SerializeField]
    private Image m_Icon;
    public  Image icon => m_Icon;
    
    [SerializeField]
    private Text m_ValueText;

    public void SetValue(BigNum value) {
        m_ValueText.text = value.ToShortForm();
    }
}
