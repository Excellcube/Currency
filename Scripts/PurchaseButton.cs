using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Excellcube;

public class PurchaseButton : MonoBehaviour
{
    [SerializeField]
    private Image m_Icon;

    [SerializeField]
    private Text m_PriceText;

    [SerializeField]
    private Text m_DisableText;

    [SerializeField]
    private Button m_Button;

    /// <summary>
    ///   구매 시 사용되는 재화 종류.
    /// </summary>
    [Header("Currency Info")]
    [SerializeField]
    private CurrencyType m_CurrencyType;
    public CurrencyType currencyType {
        get => m_CurrencyType;
        set => m_CurrencyType = value;
    }

    /// <summary>
    ///   구매를 하기 위한 가격.
    /// </summary>
    [SerializeField]
    private BigNum m_Price;
    public BigNum price {
        get => m_Price;
        set {
            m_Price = value;
            UpdateText(value);
        }
    }
    

    [Header("Events")]
    [SerializeField]
    private UnityEvent m_OnPurchase = new UnityEvent();

    [SerializeField]
    private UnityEvent m_OnFailure = new UnityEvent();



    /// <summary>
    ///   현재 버튼의 재화 설정값에 따라 재화 업데이트 시 호출되는 이벤트를 리턴.
    /// </summary>
    private UnityEvent<BigNum> onCurrencyUpdated {
        get {
            if(m_CurrencyType == CurrencyType.Gold) {
                return CurrencySystem.onGoldUpdated;
            } else {
                return CurrencySystem.onRubyUpdated;
            }
        }
    }

    /// <summary>
    ///   현재 버튼에 설정된 재화 타입에 따른 재화값.
    /// </summary>
    private BigNum currencyValue {
        get {
            if(m_CurrencyType == CurrencyType.Gold) {
                return CurrencySystem.gold;
            } else {
                return CurrencySystem.ruby;
            }
        }
        set {
            if(m_CurrencyType == CurrencyType.Gold) {
                CurrencySystem.gold = value;
            } else {
                CurrencySystem.ruby = value;
            }
        }
    }

    /// <summary>
    ///   DisableButton 메서드를 통해 버튼 비활성화를 직접 호출한 경우.
    ///   onCurrencyUpdated를 통해 버튼이 다시 활성화 되는 상황을 방지한다.
    /// </summary>
    private bool m_IsForceDisabled = false;

    private void Awake() {
        DisableButtonClickEvent();

        SetOnPurchaseListener(m_OnPurchase, m_OnFailure);
    }

    private void OnEnable() {
        onCurrencyUpdated.AddListener(UpdateButton);
        UpdateText(m_Price);
    }

    private void OnDisable() {
        onCurrencyUpdated.RemoveListener(UpdateButton);
    }

    /// <summary>
    ///   Button의 onClick에 할당된 이벤트는 실행이 되지 않도록 변경
    /// </summary>
    private void DisableButtonClickEvent()
    {
        if(m_Button == null) {
            m_Button = GetComponent<Button>();
        }

        UnityEvent buttonEvent = m_Button.onClick;
        int buttonEventsCount = buttonEvent.GetPersistentEventCount();

        if(buttonEventsCount == 0) {
            return;
        }

        for(int i=0 ; i<buttonEventsCount ; i++)
        {
            // 기존에 등록 되어 있던 이벤트는 실행되지 않도록 한다.
            buttonEvent.SetPersistentListenerState(i, UnityEventCallState.Off);
        }

        buttonEvent.AddListener(()=>{
            Debug.LogError("[Currency] PurchaseButton가 추가된 Button의 OnClick에 이벤트를 직접 할당할 수 없습니다. PurchaseButton 컴포넌트에 이벤트를 할당해주세요");
        });
    }

    private void UpdateText(BigNum price) {
        if(m_PriceText != null) {
            m_PriceText.text = price.ToShortForm();
        }
        if(m_DisableText != null) {
            m_DisableText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    ///   재화 시스템에서 재화가 갱신 되었을때 호출되는 리스너.
    /// </summary>
    private void UpdateButton(BigNum currValue) {
        if(m_IsForceDisabled) {
            return;
        }

        if(currValue >= m_Price) {
            m_Button.interactable = true;
        } else {
            m_Button.interactable = false;
        }
    }

    /// <summary>
    ///   버튼을 비활성화 하는데 호출되는 리스너. 아이콘을 비활성화 하고 버튼 라벨의 문자열을 변경한다.
    /// </summary>
    /// <param name="text"></param>
    public void DisableButton(string text) {
        m_Icon.gameObject.SetActive(false);
        m_PriceText.gameObject.SetActive(false);
        m_DisableText.gameObject.SetActive(true);
        m_DisableText.text = text;

        m_Button.interactable = false;

        m_IsForceDisabled = true;
    }

    public void SetOnPurchaseListener(UnityAction purchaseCallback, UnityAction failureCallback = null) {
        m_Button.onClick.RemoveAllListeners();

        // 버튼 클릭 시 실행되는 구메 액션 설정.
        m_Button.onClick.AddListener(() => {
            if(m_Price <= currencyValue) {
                // 구매에 성공했을 경우.
                purchaseCallback.Invoke();
                currencyValue -= m_Price;
            } else {
                // 구매에 실패했을 경우.
                failureCallback?.Invoke();
            }
        });
    }

    public void SetOnPurchaseListener(UnityEvent purchaseCallback, UnityEvent failureCallback = null) {
        m_Button.onClick.RemoveAllListeners();

        // 버튼 클릭 시 실행되는 구메 액션 설정.
        m_Button.onClick.AddListener(() => {
            if(m_Price <= currencyValue) {
                // 구매에 성공했을 경우.
                purchaseCallback.Invoke();
                currencyValue -= m_Price;
            } else {
                // 구매에 실패했을 경우.
                failureCallback?.Invoke();
            }
        });
    }
}
