using UnityEngine;
using System.Collections.Generic;

public class OBJPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected T mOrigin; // 프리팹 한 개만 받도록 변경
    [SerializeField] protected int initialPoolSize = 10; // 초기 생성 수

    protected List<T> mPool; // 단일 List로 변경

    private void Start()
    {
        mPool = new List<T>();

        // 초기 개수만큼 생성하여 풀에 추가
        for (int i = 0; i < initialPoolSize; ++i)
        {
            T newObj = MakeNewInstance();
            newObj.gameObject.SetActive(false); // 비활성화
        }
    }

    public T GetFromPool()
    {
        // 비활성화된 오브젝트를 풀에서 검색
        for (int i = 0; i < mPool.Count; ++i)
        {
            if (!mPool[i].gameObject.activeInHierarchy)
            {
                mPool[i].gameObject.SetActive(true);
                return mPool[i];
            }
        }

        // 없으면 새로 생성
        return MakeNewInstance();
    }

    protected virtual T MakeNewInstance()
    {
        T newObj = Instantiate(mOrigin);
        mPool.Add(newObj);
        return newObj;
    }

    public void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false); // 비활성화
    }
}
