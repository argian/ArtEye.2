using UnityEngine;

namespace ArtEye
{
    public class DaliHack : MonoBehaviour
    {
        private Fade _fade;

        private void Awake()
        {
            _fade = XRRigManager.Instance.XRRig.GetComponentInChildren<Fade>();

            if (!_fade)
                return;

            _fade.ForceComplete();
            _fade.SetCover(true, .5f);
        }

        private void OnDestroy()
        {
            if (_fade)
                _fade.SetCover(false, 0);
        }
    }
}
