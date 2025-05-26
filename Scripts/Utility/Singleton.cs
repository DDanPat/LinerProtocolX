using UnityEngine;

// 제네릭 싱글톤 
// <typeparam name="T">싱글톤 인스턴스로 만들 클래스 타입</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool isQuitting = false;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (isQuitting)
                return null;

            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject singletonObj = new GameObject(typeof(T).Name);
                    _instance = singletonObj.AddComponent<T>();
                    DontDestroyOnLoad(singletonObj);
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] 중복된 {typeof(T).Name} 인스턴스 제거됨", gameObject);
            Destroy(gameObject);
        }
    }
    
    //private void OnApplicationQuit()
    //{
    //    isQuitting = true;
    //}
}
