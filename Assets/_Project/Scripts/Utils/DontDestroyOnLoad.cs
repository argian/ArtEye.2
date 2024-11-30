using UnityEngine;

namespace ArtEye
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() => DontDestroyOnLoad(gameObject);
    }
}