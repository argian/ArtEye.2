using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ArtEye.Lascaux
{
    public class LightSource : XRGrabInteractable
    {
        [SerializeField] private GameObject lightSource;
        [SerializeField] private Renderer _renderer;

        private bool _keepEnabled;
        private Material _material;

        private const string _EMISSION = "_EMISSION";

        protected override void Awake()
        {
            base.Awake();

            _material = _renderer.material;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);

            if (!_keepEnabled)
                SetLight(true);
        }

        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);

            if (!_keepEnabled)
                SetLight(false);
        }

        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            
            _keepEnabled = !_keepEnabled;
        }

        private void SetLight(bool enabled)
        {
            lightSource.SetActive(enabled);

            if (enabled)
                _material.EnableKeyword(_EMISSION);
            else
                _material.DisableKeyword(_EMISSION);
        }
    }
}
