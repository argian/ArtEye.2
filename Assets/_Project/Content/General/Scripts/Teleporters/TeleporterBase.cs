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

        [field: SerializeField] public Transform TeleporterExit { get; set; }
        
        [SerializeField] protected XRBaseInteractable xrGrabInteractable;

        private Sequence sequence;
        
        protected void OnEnable() => xrGrabInteractable.selectEntered.AddListener(OnGrab);

        protected void OnDisable() => xrGrabInteractable.selectEntered.RemoveListener(OnGrab);

        private void OnGrab(SelectEnterEventArgs args) {
            if (TeleportationInProgress)
                return;

            GameObject playerObject = GetLocalPlayerObject(args);
            
            sequence = DOTween.Sequence();
            sequence.AppendCallback(() => { TeleportationInProgress = true; });
            // TODO Fade out
            sequence.Append(DOTween.To(() => 1f, value => { Debug.Log($"Fade: {value}"); }, 1f, 1f).SetEase(Ease.Linear));
            sequence.AppendCallback(async () => await Teleport(playerObject));
            // TODO Fade in
            sequence.Append(DOTween.To(() => 1f, value => { Debug.Log($"Fade: {value}"); }, 0f, 1f).SetEase(Ease.Linear));
            sequence.AppendCallback(() => { TeleportationInProgress = false; });
        }
        
        protected abstract UniTask Teleport(GameObject playerObject);

        private GameObject GetLocalPlayerObject(SelectEnterEventArgs args) => args.interactorObject.transform.GetComponentInParent<XROrigin>().gameObject;
    }
}
