using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyIconPool : MonoBehaviour
{
    [SerializeField]
    private Canvas m_Canvas;
    private Transform m_Pool;

    private static CurrencyIconPool s_Instance = null;
    public  static CurrencyIconPool Instance => s_Instance;

    private Queue<GameObject> m_GoldIconQueue = new Queue<GameObject>();
    private Queue<GameObject> m_RubyIconQueue = new Queue<GameObject>();

    private const int DEFAULT_GOLD_COUNT = 10;
    private const int DEFAULT_RUBY_COUNT = 5;

    private const float POOL_FLUSHING_INTERVAL = 10;

    private bool m_IsFetching = false;


    private void Awake()
    {
        if(m_Canvas == null)
        {
            Debug.LogError("CurrencyIconPool이 동작할 Canvas가 없습니다. CurrencySystem에 추가된 CurrencyIconPool에 Canvas를 할당해주세요");
        }
        else
        {
            s_Instance = this;

            m_Pool = (new GameObject("CurrencyIconPool")).transform;
            m_Pool.parent = m_Canvas.transform;

            // 골드 10개, 루비 5개를 미리 생성.
            for(int i=0 ; i<DEFAULT_GOLD_COUNT ; i++)
            {
                m_GoldIconQueue.Enqueue( Generate(CurrencyType.Gold) );
            }
            for(int i=0 ; i<DEFAULT_RUBY_COUNT ; i++)
            {
                m_RubyIconQueue.Enqueue( Generate(CurrencyType.Ruby) );
            }

            // POOL_FLUSHING_INTERVAL 주기로 pool의 불필요한 아이콘들을 10개씩 제거.
            InvokeRepeating(nameof(Flush), POOL_FLUSHING_INTERVAL, POOL_FLUSHING_INTERVAL);
        }
    }

    private GameObject Generate(CurrencyType type)
    {
        GameObject iconPrefab = CurrencySystem.GetIconPrefab(type);
        iconPrefab.gameObject.SetActive(false);
        return Instantiate(iconPrefab, Vector3.zero, Quaternion.identity, m_Pool);
    }

    public GameObject Fetch(CurrencyType type)
    {
        m_IsFetching = true;

        var iconQueue = GetIconQueue(type);
        GameObject icon;

        // 큐에서 아이템을 하나 가져온다.
        if(!iconQueue.TryDequeue(out icon)) {
            // 큐가 비어있을 경우 10개의 아이템을 더 생성
            for(int i=0 ; i<10 ; i++) {
                iconQueue.Enqueue( Generate(type) );
            }
            icon = iconQueue.Dequeue();
        }

        icon.gameObject.SetActive(true);

        m_IsFetching = false;
        return icon;
    }

    public void Release(CurrencyType type, GameObject icon)
    {
        icon.gameObject.SetActive(false);

        var iconQueue = GetIconQueue(type);
        iconQueue.Enqueue(icon);
    }

    private Queue<GameObject> GetIconQueue(CurrencyType type)
    {
        if(type == CurrencyType.Gold)
        {
            return m_GoldIconQueue;
        }
        else
        {
            return m_RubyIconQueue;
        }
    }

    private void Flush()
    {
        Flush(CurrencyType.Gold);
        Flush(CurrencyType.Ruby);
    }

    private void Flush(CurrencyType type)
    {
        if(m_IsFetching) {
            return;
        }

        int minIconCount = 10;
        var iconQueue = GetIconQueue(type);

        if(iconQueue.Count > minIconCount)
        {
            int removalsCount = iconQueue.Count - minIconCount;
            for(int i=0 ; i<removalsCount ; i++)
            {
                GameObject iconGo = iconQueue.Dequeue();
                Destroy(iconGo);
            }
        }
    }
}
