using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ArtEye
{
    public class InteractionMarker : XRSimpleInteractable
    {
        [SerializeField] private float spinningSpeed = 45f;

        private float _currentSpinningSpeed;
        private Tweener _clickAnimation;

        protected override void Awake()
        {
            base.Awake();
            _clickAnimation = transform.DOScale(new Vector3(.8f, .8f, .8f), 0.3f)
                .SetEase(Ease.OutBounce)
                .Pause()
                .SetAutoKill(false);
        }

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

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _clickAnimation.Restart();
        }
        
        private void Spin()
        {
            if (!Mathf.Approximately(_currentSpinningSpeed, spinningSpeed))
                _currentSpinningSpeed = Mathf.Lerp(_currentSpinningSpeed, spinningSpeed, .1f);
            
            transform.Rotate(Vector3.up, _currentSpinningSpeed * Time.deltaTime);
        }
    }
}
