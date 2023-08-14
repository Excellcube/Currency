using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Excellcube;
using RadiusOne.Currency;

/// 
/// 외부 접근 편의성을 위해 namespace 제거.
/// 

// 게임 시스템의 초기화 이전에 CurrencySystem의 Awake가 실행되어 CurrencySystem의 인스턴스를 참조할 수 있어야 한다.
[DefaultExecutionOrder(-10000)]
public class CurrencySystem : MonoBehaviour {
    /// 
    ///  외부 접근 편의성을 위한 static 필드 영역.
    /// 
    private static CurrencyModel m_Model = new CurrencyModel();

    public static BigNum gold {
        get {
            return m_Model.gold;
        }
        set {
            if(s_System.m_GoldField != null) {
                s_System.m_GoldField.SetValue(value);
            } else {
                Debug.LogWarning("GoldField에 UI 컴포넌트가 할당되지 않았음");
            }
            
            s_System.m_OnGoldUpdated.Invoke(value);
            m_Model.gold = value;
        }
    }

    public static BigNum ruby {
        get {
            return m_Model.ruby;
        }
        set {
            if(s_System.m_RubyField != null) {
                s_System.m_RubyField.SetValue(value);
            } else {
                Debug.LogWarning("RubyField에 UI 컴포넌트가 할당되지 않았음");
            }

            s_System.m_OnRubyUpdated.Invoke(value);
            m_Model.ruby = value;
        }
    }

    private static CurrencySystem s_System = null;


    /// 
    ///  instance 필드 영역.
    /// 
    [Header("UI")]
    [SerializeField]
    private GoldField m_GoldField;

    [SerializeField]
    private RubyField m_RubyField;

    [Header("Events")]
    [SerializeField]
    private UnityEvent<BigNum> m_OnGoldUpdated = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onGoldUpdated => s_System.m_OnGoldUpdated;

    [SerializeField]
    private UnityEvent<BigNum> m_OnRubyUpdated = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onRubyUpdated => s_System.m_OnRubyUpdated;


    private void Awake() {
        s_System = this;
    }
}