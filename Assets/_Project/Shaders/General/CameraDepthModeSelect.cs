using UnityEngine;


namespace ArtEye
{
    [RequireComponent(typeof(Camera))]
    public class CameraDepthModeSelect : MonoBehaviour
    {
        public DepthTextureMode mode;
        void Start()
        {
            gameObject.GetComponent<Camera>().depthTextureMode = mode;
        }
    }
}
