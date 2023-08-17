using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Excellcube;

public class GoldField : MonoBehaviour, ICurrencyField {
    [SerializeField]
    private Image m_Icon;


    [SerializeField]
    private Text m_ValueText;

    public void SetValue(BigNum value) {
        m_ValueText.text = value.ToShortForm();
    }

    public Image GetIcon() {
        return m_Icon;
    }
}
