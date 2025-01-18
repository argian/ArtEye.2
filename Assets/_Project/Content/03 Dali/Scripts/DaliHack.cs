using UnityEngine;

namespace ArtEye
{
    public class DaliHack : MonoBehaviour
    {
        private Fade _fade;

        private void Awake()
        {
            _fade = XRRigManager.Instance.XRRig.GetComponentInChildren<Fade>();

            if (_fade)
                _fade.SetFade(true, .5f);
        }

        private void OnDestroy()
        {
            if (_fade)
                _fade.SetFade(false, 0);
        }
    }
}
