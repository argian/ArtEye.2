using UnityEngine;

namespace ArtEye
{
    public class ToggleDepthHack : MonoBehaviour
    {
        private const string _SSAO = "ScreenSpaceAmbientOcclusion";

        private void Awake()
        {
            UniversalRenderPipelineUtils.SetRendererFeatureActive(_SSAO, true);
        }

        private void OnDestroy()
        {
            UniversalRenderPipelineUtils.SetRendererFeatureActive(_SSAO, false);
        }
    }
}