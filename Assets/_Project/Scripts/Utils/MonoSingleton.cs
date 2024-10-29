using UnityEngine;

namespace ArtEye
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        public static T Instance { get; private set; }

        [SerializeField] private bool dontDestroyOnLoad;

        protected virtual void Awake()
        {
            if (Instance == null)
            {
                Instance = GetComponent<T>();

                if (dontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (Instance = GetComponent<T>())
                Instance = null;
        }
    }
}