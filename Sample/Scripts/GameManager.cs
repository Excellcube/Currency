using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.Currency.Sample
{
    public class GameManager : MonoBehaviour
    {
        void Start()
        {
            CurrencySystem.gold = 500;      
            CurrencySystem.ruby = 10; 
        }
    }
}
