using UnityEngine;

namespace ArtEye.TheLastSupper
{
    public class DimensionalLens : MonoBehaviour
    {
        [SerializeField] private MeshRenderer portalBaker;

        public MeshRenderer GetRenderer()
        {
            return portalBaker;
        }
    }
}
