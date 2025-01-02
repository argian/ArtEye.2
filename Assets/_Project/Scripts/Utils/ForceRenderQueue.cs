using UnityEngine;

namespace ArtEye
{
    public class ForceRenderQueue : MonoBehaviour
    {
        [SerializeField]
        protected Material material;
        [SerializeField]
        protected int renderQueue;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            material.renderQueue = renderQueue;
        }
    }
}
