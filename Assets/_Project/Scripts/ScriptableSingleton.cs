using UnityEngine;

namespace ArtEye
{
    public abstract class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
    {
        private static T _instance;

        public static T Instance 
        { 
            get 
            {
                if (_instance == null)
                    _instance = Resources.Load(typeof(T).Name) as T;

                return _instance;
            }
        }
    }
}
