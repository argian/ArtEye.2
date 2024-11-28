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
        private Sequence _clickAnimation;

        protected override void Awake()
        {
            base.Awake();
            _clickAnimation = DOTween.Sequence()
                .Append(transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.3f).SetEase(Ease.OutExpo))
                .Append(transform.DOScale(new Vector3(1f, 1f, 1f), 0.3f).SetEase(Ease.InOutSine))
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
