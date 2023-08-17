using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Excellcube;

public class CurrencySpreader : MonoBehaviour
{
    private struct MoveIconsParam
    {
        public RectTransform start;
        public RectTransform end;
        public List<Transform> icons;
        public UnityAction onArrivalFirst;
        public UnityAction onArrivalLast;
    }


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
    // private List<Transform> m_Icons = new List<Transform>();

    private void Awake()
    {
        m_Canvas = GetComponentInParent<Canvas>();
    }

    /// <summary>
    /// 애니메이션 효과 없이 값을 즉시 증가 시킨다.
    /// </summary>
    public void AddCurrencyWithoutAnimation()
    {
        // 애니메이션 없이 즉시 재화 증가.
        CurrencySystem.Add(m_Type, m_Price);
    }

    /// <summary>
    /// CurrencySpreader가 포함된 GameObject부터 CurrencySystem에 등록된 GoldField, RubyField의 아이콘까지 애니메이션 효과를 통해 이동한다.
    /// </summary>
    public void AddCurrencyWithAnimation(int iconCount = 10)
    {
        // type에 맞는 아이콘 N개를 생성
        // GameObject iconPrefab = CurrencySystem.GetIconPrefab(m_Type);
        Vector3 position = transform.position;
        float seed = 20;

        // 재화 아이콘들을 생성.
        // CurrencySpreader가 포함된 GameObject를 기준으로 
        // 주변에 아이콘들을 랜덤하게 배치한다.
        List<Transform> icons = new List<Transform>();

        for(int i=0 ; i<iconCount ; i++)
        {
            Vector3 randPosition = new Vector3();
            randPosition.x = position.x + Random.Range(-seed, seed);
            randPosition.y = position.y + Random.Range(-seed, seed);
            randPosition.z = position.z;

            // GameObject iconObj = Instantiate(iconPrefab, randPosition, Quaternion.identity, m_Canvas.transform);
            GameObject iconObj = CurrencyIconPool.Instance.Fetch(m_Type);
            iconObj.transform.position = randPosition;
            icons.Add(iconObj.transform);
        }


        // 아이콘 이동 애니메이션에 관련된 매개변수들 설정.
        MoveIconsParam param = new MoveIconsParam();

        param.start = GetComponent<RectTransform>();;
        param.end   = CurrencySystem.GetDestination(m_Type);
        param.icons = icons;
        param.onArrivalFirst = ()=>{
            // 첫 번째 아이콘이 목적지에 도착할 시
            // 카운팅 효과로 금액이 올라가는 애니메이션 실행.
            CurrencySystem.Add(m_Type, m_Price, withCounting: true);
        };
        param.onArrivalLast  = ()=>{
            // 마지막 아이콘이 목적지에 도착할 시
            // 관리 중인 아이콘 배열 제거.
            icons.Clear();
        };

        // 현재 위치에서 목적지까지 이동.
        MoveIcons(param);
    }

    private void MoveIcons(MoveIconsParam param)
    {
        StartCoroutine( MoveIconsInternal(param) );
    }

    private IEnumerator MoveIconsInternal(MoveIconsParam param)
    {
        // 가장 위에 보이는 아이콘이 먼저 움직이게 하기 위해
        // 아이콘이 추가된 순서의 역순으로 애니메이션 실행.
        for(int i=param.icons.Count - 1 ; i>=0 ; i--)
        {
            IconMover mover = param.icons[i].GetComponent<IconMover>();
            mover.Move(param.end.transform, 1.0f, ()=>{
                // 이동이 완료 됐을 경우 pool에 반환.
                CurrencyIconPool.Instance.Release(m_Type, mover.gameObject);
            });

            mover.onFinish.RemoveAllListeners();

            if(i == param.icons.Count - 1) {
                // 첫 번째 아이콘이 목적지에 도착했을때 실행할 이벤트 등록.
                mover.onFinish.AddListener(param.onArrivalFirst);
            }

            if(i == 0) {
                // 마지막 아이콘이 목적지에 도착했을때 실행할 이벤트 등록.
                mover.onFinish.AddListener(param.onArrivalLast);
            }

            yield return new WaitForSeconds(0.03f);
        }
    }
}
