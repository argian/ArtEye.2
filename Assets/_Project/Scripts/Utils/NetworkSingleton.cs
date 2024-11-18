using Unity.Netcode;
using UnityEngine;

namespace ArtEye
{
    public class NetworkSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
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

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (Instance = GetComponent<T>())
                Instance = null;
        }
    }
}