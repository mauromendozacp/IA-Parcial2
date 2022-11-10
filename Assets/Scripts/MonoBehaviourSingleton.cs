using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : Component
{
    #region PROTECTED_FIELDS
    protected static T instance = null;
    #endregion

    #region PROPERTIES
    public static T Instance => instance;
    #endregion

    #region UNITY_CALLS
    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
}