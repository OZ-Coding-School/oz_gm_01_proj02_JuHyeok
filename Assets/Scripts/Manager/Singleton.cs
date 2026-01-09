using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T inst;

    public static T Instance
    {
        get
        {
            //인스턴스가 없으면 생성하자
            if (inst == null)
            {
                //씬 안에서 같은 타입의 컴포넌트를 탐색
                inst = (T)FindFirstObjectByType(typeof(T));

                //그래도 없으면 새로운 게임오브젝트를 만들어서 생성
                if (inst == null)
                {
                    GameObject go = new GameObject();
                    inst = go.AddComponent<T>();

                    go.name = typeof(T).Name;

                    DontDestroyOnLoad(go);
                }
            }
            return inst;
        }
    }
    public virtual void Awake()
    {
        if (inst == null)
        {
            inst = this as T;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }
}
