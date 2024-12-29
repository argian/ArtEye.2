using DG.Tweening;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ArtEye
{
    public class InteractionMarker : XRSimpleInteractable
    {
        [SerializeField] private float spinningSpeed = 45f;
        [SerializeField] private float hoverSpeedMultiplier = 8f;

        private Sequence _clickAnimation;
        private float _targetSpinningSpeed;
        private float _currentSpinningSpeed;
        private int _hovered;
        
        protected override void Awake()
        {
            base.Awake();
            _clickAnimation = DOTween.Sequence()
                .Append(transform.DOScale(transform.localScale * .8f, 0.3f).SetEase(Ease.OutExpo))
                .Append(transform.DOScale(transform.localScale, 0.3f).SetEase(Ease.InOutSine))
                .Pause()
                .SetAutoKill(false);
            _targetSpinningSpeed = spinningSpeed;
        }

        private void Update()
        {
            Spin();
        }
        
        protected override void OnHoverEntered(HoverEnterEventArgs args)
        {
            base.OnHoverEntered(args);
            _hovered++;
            if (_hovered > 1)
                return;
            _targetSpinningSpeed *= hoverSpeedMultiplier;
        }

        protected override void OnHoverExited(HoverExitEventArgs args)
        {
            base.OnHoverExited(args);
            _hovered--;
            if (_hovered == 0)
                _targetSpinningSpeed = spinningSpeed;
            if (_hovered < 0)
                _hovered = 0;
        }

        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            _clickAnimation.Restart();
        }
        
        private void Spin()
        {
            if (!Mathf.Approximately(_currentSpinningSpeed, _targetSpinningSpeed))
                _currentSpinningSpeed = Mathf.Lerp(_currentSpinningSpeed, _targetSpinningSpeed, .1f);
            
            transform.Rotate(Vector3.up, _currentSpinningSpeed * Time.deltaTime);
        }
    }
}
