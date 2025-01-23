using UnityEngine;
using System.Collections.Generic;

public class OBJPool<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected T mOrigin; // ������ �� ���� �޵��� ����
    [SerializeField] protected int initialPoolSize = 10; // �ʱ� ���� ��

    protected List<T> mPool; // ���� List�� ����

    private void Start()
    {
        mPool = new List<T>();

        // �ʱ� ������ŭ �����Ͽ� Ǯ�� �߰�
        for (int i = 0; i < initialPoolSize; ++i)
        {
            T newObj = MakeNewInstance();
            newObj.gameObject.SetActive(false); // ��Ȱ��ȭ
        }
    }

    public T GetFromPool()
    {
        // ��Ȱ��ȭ�� ������Ʈ�� Ǯ���� �˻�
        for (int i = 0; i < mPool.Count; ++i)
        {
            if (!mPool[i].gameObject.activeInHierarchy)
            {
                mPool[i].gameObject.SetActive(true);
                return mPool[i];
            }
        }

        // ������ ���� ����
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
        obj.gameObject.SetActive(false); // ��Ȱ��ȭ
    }
}
