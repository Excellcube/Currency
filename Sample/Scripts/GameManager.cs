using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.Currency.Sample
{
    public class GameManager : MonoBehaviour
    {
        void Awake()
        {
            CurrencySystem.gold = 500;      
            CurrencySystem.ruby = 10; 
        }

        /// 
        ///  Currency System 이벤트.
        /// 
        
        public void onGoldUpdated(BigNum value)
        {
            Debug.Log("Gold updated : " + value.ToShortForm());
        }

        public void onRubyUpdated(BigNum value)
        {
            Debug.Log("Ruby updated : " + value.ToShortForm());
        }


        /// 
        ///  Currency Button 이벤트.
        /// 

        public void PurchaseGoldItem()
        {
            Debug.Log("Purchase Gold item!");
        }

        public void PurchaseRubyItem()
        {
            Debug.Log("Purchase Ruby item!");
        }


        ///
        ///  Add Currency 이벤트.
        ///

        public void AddGoldWithNoAnimation(int value)
        {
            CurrencySystem.gold += value;
        }

        public void AddRubyWithNoAnimation(int value)
        {
            CurrencySystem.ruby += value;
        }
    }
}
