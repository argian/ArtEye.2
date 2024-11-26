using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ArtEye.Teleportation
{
    public abstract class TeleporterBase : MonoBehaviour
    {
        private static bool TeleportationInProgress { get; set; }
        
        [SerializeField] protected XRBaseInteractable xrGrabInteractable;
        
        protected void OnEnable() => xrGrabInteractable.selectEntered.AddListener(OnGrab);

        protected void OnDisable() => xrGrabInteractable.selectEntered.RemoveListener(OnGrab);
        
        protected abstract UniTask Teleport(GameObject playerObject);

        private void OnGrab(SelectEnterEventArgs args)
        {
            if (TeleportationInProgress)
                return;
            
            Teleportation(GetLocalPlayerObject(args)).Forget();
        }

        private async UniTaskVoid Teleportation(GameObject playerObject)
        {
            TeleportationInProgress = true;
            // TODO Fade out
            // await playerObject.FadeOut();
            await Teleport(playerObject);
            // TODO Fade in
            // await playerObject.FadeIn();
            TeleportationInProgress = false;
        }

        private GameObject GetLocalPlayerObject(SelectEnterEventArgs args)
        {
            // TODO access local player or find something like Player component in hierarchy
            return args.interactorObject.transform.GetComponentInParent<XROrigin>().gameObject;
        }
    }
}
