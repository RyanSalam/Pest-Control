using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                    _instance = new GameObject("Instance of: " + (typeof(T))).AddComponent<T>();
            }

            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
            _instance = GetComponent<T>();

        if (Instance != this)
            Destroy(gameObject);
    }
}
