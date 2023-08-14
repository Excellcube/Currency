using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Excellcube;

namespace RadiusOne.Currency {
    /// <summary>
    /// Currency의 데이터를 다루는 클래스. 외부로 노출시킬 필요가 없기 때문에 internal로 처리.
    /// </summary>
    internal class CurrencyModel
    {
        private BigNum m_Gold;
        public BigNum gold {
            get {
                return m_Gold;
            }
            set {
                m_Gold = value;
            }
        }

        private BigNum m_Ruby;
        public BigNum ruby {
            get {
                return m_Ruby;
            }
            set {
                m_Ruby = value;
            }
        }
    }
}