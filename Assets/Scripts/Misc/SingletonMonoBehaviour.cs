using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    // allow other classes to incherit Awake() and make is possible to overwrite it
    protected virtual void Awake()
    {
        // check for null if it is assign to _instance
        if (_instance == null)
        {
            _instance = this as T;
            //DontDestroyOnLoad(gameObject);
        }
        // make sure there is only one instance of this class
        else
        {
            Destroy(gameObject);
        }
    }
}