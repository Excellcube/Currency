using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Excellcube.Currency.Sample
{
    public class GameManager : MonoBehaviour
    {
        void Awake()
        {
            CurrencySystem.Set(CurrencyType.Gold, 500);
            CurrencySystem.Set(CurrencyType.Ruby, 10);
        }

        /// 
        ///  Currency System 이벤트.
        /// 
        
        public void onGoldAdded(BigNum value)
        {
            Debug.Log("Gold added : " + value.ToShortForm());
        }

        public void onGoldUpdated(BigNum value)
        {
            Debug.Log("Gold updated : " + value.ToShortForm());
        }

        public void onGoldUsed(BigNum value)
        {
            Debug.Log("Gold used : " + value.ToShortForm());
        }

        public void onRubyAdded(BigNum value)
        {
            Debug.Log("Ruby added : " + value.ToShortForm());
        }

        public void onRubyUpdated(BigNum value)
        {
            Debug.Log("Ruby updated : " + value.ToShortForm());
        }

        public void onRubyUsed(BigNum value)
        {
            Debug.Log("Ruby used : " + value.ToShortForm());
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
    }
}
