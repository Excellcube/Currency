using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Excellcube;
using RadiusOne.Currency;

/// 
/// 외부 접근 편의성을 위해 namespace 제거.
/// 

// 게임 시스템의 초기화 이전에 CurrencySystem의 Awake가 실행되어 CurrencySystem의 인스턴스를 참조할 수 있어야 한다.
[ExecuteInEditMode]
[DefaultExecutionOrder(-10000)]
public class CurrencySystem : MonoBehaviour {
    /// 
    ///  외부 접근 편의성을 위한 static 필드 영역.
    /// 
    private static CurrencyModel m_Model = new CurrencyModel();

    /// <summary>
    /// 시스템 초기화를 진행했는지 여부. 외부 클래스에서 CurrencySystem.gold = {값}과 같이 재화를 초기화 해야 한다.
    /// </summary>
    private static bool m_IsInitialized = false;

    public static BigNum gold {
        get {
            return m_Model.gold;
        }
    }

    public static BigNum ruby {
        get {
            return m_Model.ruby;
        }
    }

    private static CurrencySystem s_System = null;


    /// 
    ///  instance 필드 영역.
    /// 
    [Header("UI")]
    [SerializeField]
    private GoldField m_GoldField;
    static public GoldField goldField => s_System.m_GoldField;

    [SerializeField]
    private RubyField m_RubyField;
    static public RubyField rubyField => s_System.m_RubyField;

    [Header("Icons")]
    [SerializeField]
    private GameObject m_GoldIconPrefab;
    static public GameObject goldIconPrefab => s_System.m_GoldIconPrefab;

    [SerializeField]
    private GameObject m_RubyIconPrefab;
    static public GameObject rubyIconPrefab => s_System.m_RubyIconPrefab;

    [Header("Events")]
    [SerializeField]
    private UnityEvent<BigNum> m_OnGoldAdded = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onGoldAdded => s_System.m_OnGoldAdded;
    [SerializeField]
    private UnityEvent<BigNum> m_OnGoldUpdated = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onGoldUpdated => s_System.m_OnGoldUpdated;
    [SerializeField]
    private UnityEvent<BigNum> m_OnGoldUsed = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onGoldUsed => s_System.m_OnGoldUsed;

    [SerializeField]
    private UnityEvent<BigNum> m_OnRubyAdded = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onRubyAdded => s_System.m_OnRubyAdded;
    [SerializeField]
    private UnityEvent<BigNum> m_OnRubyUpdated = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onRubyUpdated => s_System.m_OnRubyUpdated;
    [SerializeField]
    private UnityEvent<BigNum> m_OnRubyUsed = new UnityEvent<BigNum>();
    static public UnityEvent<BigNum> onRubyUsed => s_System.m_OnRubyUsed;


    private void Awake() {
        s_System = this;
    }

    private void Start() {
        // if(!m_IsInitialized) {
        //     Debug.LogError("[Currency] CurrencySystem이 초기화 되지 않았음.");
        //     Debug.LogError("[Currency] 외부의 Awake 이벤트에서 CurrencySystem.gold = 100과 같은 방법으로 초기화를 해야함");
        // }
    }

    static public void Set(CurrencyType type, BigNum value)
    {
        ICurrencyField field = GetCurrencyField(type);
        field.SetValue(value);

        UnityEvent<BigNum> onUpdated = GetOnUpdatedEvent(type);
        onUpdated.Invoke(value);

        SetModelData(type, value);

        m_IsInitialized = true;
    }

    static public void Add(CurrencyType type, BigNum addedValue, bool withCounting = false) {
        if(withCounting)
        {
            AddWithCounting(type, addedValue);
        }
        else
        {
            AddImmediately(type, addedValue);
        }
    }

    static private void AddImmediately(CurrencyType type, BigNum addedValue) {
        BigNum prevValue = GetModelData(type);
        BigNum newValue  = prevValue + addedValue;
        
        Set(type, newValue);

        var onAddedEvent = GetOnAddedEvent(type);
        onAddedEvent?.Invoke(addedValue);
    }

    /// <summary>
    ///   카운팅이 되면서 금액 증가.
    /// </summary>
    static private void AddWithCounting(CurrencyType type, BigNum addedValue) {
        // 카운팅 효과와 함께 증가.
        s_System.StartCoroutine( PointCounterUp(type, addedValue) );

        // CurrencyField의 업데이트 없이
        // Model 정보만 업데이트.
        BigNum prevValue = GetModelData(type);
        BigNum newValue  = prevValue + addedValue;

        UnityEvent<BigNum> onUpdated = GetOnUpdatedEvent(type);
        onUpdated.Invoke(newValue);

        SetModelData(type, newValue);
    }

    static public void Use(CurrencyType type, BigNum usedValue) {
        BigNum prevValue = GetModelData(type);
        BigNum newValue = prevValue - usedValue;

        Set(type, newValue);

        var onUsedEvent = GetOnUsedEvent(type);
        onUsedEvent?.Invoke(usedValue);
    }

    private static IEnumerator PointCounterUp(CurrencyType type, BigNum addedValue, UnityAction finishAction = null)
    {
        BigNum prevValue = type == CurrencyType.Gold ? gold : ruby;
        BigNum targetValue = (prevValue + addedValue).ToDouble();
        
        float  countingDuration = 2.0f;      // 2초동안 카운팅.
        int    framePerSecond = 30;
        int    countFrames = Mathf.RoundToInt(framePerSecond * countingDuration);

        // 매 프레임마다 증가시킬 값의 스텝 계산.
        double coinStep = addedValue.ToDouble() / countFrames;
        if (coinStep == 0)
        {
            coinStep = 1;
        }

        double currValue = prevValue.ToDouble();

        // 카운팅을 하면서 field 값 업데이트.
        while (currValue < targetValue)
        {
            currValue += coinStep;

            // overflow된 값이 있을 경우 ceiling 효과.
            if (currValue > targetValue)
            {
                currValue = targetValue.ToDouble();
            }

            ICurrencyField field = GetCurrencyField(type);
            field.SetValue(currValue);

            yield return null;
        }

        finishAction?.Invoke();
    }

    public static UnityEvent<BigNum> GetOnAddedEvent(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            return onGoldAdded;
        }
        else
        {
            return onRubyAdded;
        }
    }

    public static UnityEvent<BigNum> GetOnUpdatedEvent(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            return onGoldUpdated;
        }
        else
        {
            return onRubyUpdated;
        }
    }

    public static UnityEvent<BigNum> GetOnUsedEvent(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            return onGoldUsed;
        }
        else
        {
            return onRubyUsed;
        }
    }

    private static void SetModelData(CurrencyType type, BigNum value)
    {
        if(type == CurrencyType.Gold)
        {
            m_Model.gold = value;
        }
        else
        {
            m_Model.ruby = value;
        }
    }

    private static BigNum GetModelData(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            return m_Model.gold;
        }
        else
        {
            return m_Model.ruby;
        }
    }

    public static GameObject GetIconPrefab(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            return CurrencySystem.goldIconPrefab;
        }
        else
        {
            return CurrencySystem.rubyIconPrefab;
        }
    }

    public static RectTransform GetDestination(CurrencyType type)
    {
        ICurrencyField field = GetCurrencyField(type);
        Image icon = field.GetIcon();        
        return icon.GetComponent<RectTransform>();
    }

    public static ICurrencyField GetCurrencyField(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            return CurrencySystem.goldField;
        }
        else
        {
            return CurrencySystem.rubyField;
        }
    }
}