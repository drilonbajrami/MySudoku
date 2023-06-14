using UnityEngine;

namespace MySudoku
{
    /// <summary>
    /// Generic Singleton class pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        private static T instance;

        /// <summary>
        /// Singleton instance getter.<br/>
        /// If there is no instance, try find one in the scene.<br/>
        /// If still there no instance found, then it creates a new one.
        /// </summary>
        public static T Instance {
            get {
                if (instance == null)
                    instance = FindObjectOfType<T>();

                if (instance == null) {
                    GameObject obj = new() { name = typeof(T).Name };
                    instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                }

                return instance;
            }
        }

        /// <summary>
        /// If no singleton instance is found then assign this one as the only instance,<br/>
        /// else destroy it.
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null) {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }
    }
}