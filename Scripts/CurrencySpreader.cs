using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Excellcube;

public class CurrencySpreader : MonoBehaviour
{
    [SerializeField]
    private CurrencyType m_Type;
    public CurrencyType type {
        get => m_Type;
        set => m_Type = value;
    }

    [SerializeField]
    private BigNum m_Price;
    public  BigNum price {
        get => m_Price;
        set => m_Price = value;
    }

    private Canvas m_Canvas;
    private List<Transform> m_Icons = new List<Transform>();

    private void Awake()
    {
        m_Canvas = GetComponentInParent<Canvas>();
    }

    public void AddCurrencyWithAnimation()
    {
        // type에 맞는 아이콘 N개를 생성
        GameObject iconPrefab = GetIconPrefab(m_Type);
        int iconsCount = 10;
        Vector3 position = transform.position;

        for(int i=0 ; i<iconsCount ; i++)
        {
            Vector3 randPosition = new Vector3();
            randPosition.x = position.x + Random.Range(-20, 20);
            randPosition.y = position.y + Random.Range(-20, 20);
            randPosition.z = position.z;

            GameObject iconObj = Instantiate(iconPrefab, randPosition, Quaternion.identity, m_Canvas.transform);
            m_Icons.Add(iconObj.transform);
        }


        // 현재 위치와 목적지의 위치 확인.
        RectTransform start = GetComponent<RectTransform>();
        RectTransform destination = GetDestination(m_Type);

        // 현재 위치에서 목적지까지 이동.
        MoveIcons(start, destination, ()=>{
            Debug.Log("Count up!");
        }, ()=>{
            m_Icons.Clear();
        });


        // 목적지에 도착한 아이콘은 애니메이션 혹은 파티클 실행.

        // 값이 증가하는 텍스트 효과.

    }

    private GameObject GetIconPrefab(CurrencyType type)
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

    private RectTransform GetDestination(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            GoldField goldField = CurrencySystem.goldField;
            Image icon = goldField.icon;
            return icon.GetComponent<RectTransform>();
        }
        else
        {
            RubyField rubyField = CurrencySystem.rubyField;
            Image icon = rubyField.icon;
            return icon.GetComponent<RectTransform>();
        }
    }

    private void MoveIcons(RectTransform start, RectTransform destination, UnityAction onArrivalFirst, UnityAction onArrivalLast)
    {
        StartCoroutine( MoveIconsInternal(start, destination, onArrivalFirst, onArrivalLast) );
    }

    private IEnumerator MoveIconsInternal(RectTransform start, RectTransform destination, UnityAction onArrivalFirst, UnityAction onArrivalLast)
    {
        // 가장 위에 보이는 아이콘이 먼저 움직이게 하기 위해
        // 아이콘이 추가된 순서의 역순으로 애니메이션 실행.
        for(int i=m_Icons.Count - 1 ; i>=0 ; i--)
        {
            IconMover mover = m_Icons[i].GetComponent<IconMover>();
            mover.Move(destination.transform, 1.0f);

            if(i == m_Icons.Count - 1) {
                // 첫 번째 아이콘이 목적지에 도착했을때 실행할 이벤트 등록.
                mover.onFinish.AddListener(onArrivalFirst);
            }

            if(i == 0) {
                // 마지막 아이콘이 목적지에 도착했을때 실행할 이벤트 등록.
                mover.onFinish.AddListener(onArrivalLast);
            }

            yield return new WaitForSeconds(0.03f);
        }
    }

    public void AddCurrencyWithoutAnimation()
    {
        // 애니메이션 없이 즉시 재화 증가.
        if(m_Type == CurrencyType.Gold)
        {
            CurrencySystem.gold += m_Price;
        }
        else
        {
            CurrencySystem.ruby += m_Price;
        }
    }
}
