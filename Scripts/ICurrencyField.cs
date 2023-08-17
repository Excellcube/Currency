using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Excellcube;

public interface ICurrencyField
{
    public void SetValue(BigNum value);
    public Image GetIcon();
}
