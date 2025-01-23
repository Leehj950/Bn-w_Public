
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] bool isDDO = false;
    private static T instance;

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                Debug.Log("Singleton 객체가 없습니다.");
                return null;
            }

            return instance;
        }
    }

    public virtual void Awake()
    {
        if(instance == null)
        {
            instance = this.GetComponent<T>();
        }

        if(isDDO)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
