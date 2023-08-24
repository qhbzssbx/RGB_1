using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = GameObject.Find(typeof(T).Name);
                if (go == null)
                {
                    go = AssetManager.LoadUIPrefab(typeof(T).Name);
                }
                if (go == null)
                {
                    go = new GameObject(typeof(T).Name);
                }
                try
                {
                    instance = go.AddComponent<T>();
                }
                catch
                {

                    Debug.Log(typeof(T).Name);
                }
                
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    /// <summary>
    /// ���ٵ���
    /// </summary>
    public virtual void Discard()
    {
        Destroy(gameObject);
        instance = null;
    }
}
