using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ArtEye
{
    public class InteractionMarker : XRSimpleInteractable
    {
        [SerializeField] private float spinningSpeed = 45f;

        private float _currentSpinningSpeed;
        
        private void Update()
        {
            Spin();
        }

        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            spinningSpeed *= 8f;
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            spinningSpeed /= 8f;
        }

        private void Spin()
        {
            if (!Mathf.Approximately(_currentSpinningSpeed, spinningSpeed))
                _currentSpinningSpeed = Mathf.Lerp(_currentSpinningSpeed, spinningSpeed, .1f);
            
            transform.Rotate(Vector3.up, _currentSpinningSpeed * Time.deltaTime);
        }
    }
}
