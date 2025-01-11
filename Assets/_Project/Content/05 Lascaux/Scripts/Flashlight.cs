using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ArtEye.Lascaux
{
    public class Flashlight : XRGrabInteractable
    {

        [SerializeField] private GameObject lightSource;
        private bool _isOn;
        
        protected override void OnActivated(ActivateEventArgs args)
        {
            base.OnActivated(args);
            _isOn = !_isOn;
            lightSource.SetActive(_isOn);
        }
    }
}
