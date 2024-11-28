using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ArtEye.Teleportation
{
    public abstract class TeleporterBase : MonoBehaviour
    {
        public static Action<Action> OnTeleportationStart = delegate { };
        public static Action OnTeleportationEnd = delegate { };

        private static bool TeleportationInProgress { get; set; }
        
        [SerializeField] protected XRBaseInteractable xrGrabInteractable;
        
        protected virtual void OnEnable() => xrGrabInteractable.selectEntered.AddListener(OnGrab);

        protected virtual void OnDisable() => xrGrabInteractable.selectEntered.RemoveListener(OnGrab);
        
        protected abstract void Teleport(GameObject playerObject);

        private void OnGrab(SelectEnterEventArgs args)
        {
            if (TeleportationInProgress)
                return;
            
            OnTeleportationStart?.Invoke(() => { Teleport(XRRigManager.Instance.XRRig); });
        }
    }
}
