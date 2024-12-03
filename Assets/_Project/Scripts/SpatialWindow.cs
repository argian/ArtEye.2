using DG.Tweening;
using UnityEngine;

namespace ArtEye
{
    public class SpatialWindow : MonoBehaviour
    {
        public bool IsOpen
        {
            get => isOpen;
            set
            {
                if (isOpen == value)
                    return;
                
                if (value)
                {
                    _openAnimation.PlayForward();
                    _fadeAnimation.PlayForward();
                }
                else
                {
                    _openAnimation.PlayBackwards();
                    _fadeAnimation.PlayBackwards();
                }

                isOpen = value;
            }
        }

        [SerializeField] private RectTransform window;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private bool isOpen;
        
        private Tweener _openAnimation;
        private Tweener _fadeAnimation;

        private void Awake()
        {
            Vector2 windowSize = window.sizeDelta;
            
            _openAnimation = window.DOSizeDelta(windowSize, 1f)
                .From(Vector3.zero)
                .SetEase(Ease.OutSine)
                .Pause()
                .SetAutoKill(false);
            
            _fadeAnimation = canvasGroup.DOFade(1f, .3f)
                .From(0f)
                .SetEase(Ease.InOutSine)
                .Pause()
                .SetAutoKill(false);

            window.sizeDelta = windowSize;
            canvasGroup.alpha = IsOpen ? 1f : 0f;
            if (IsOpen)
            {
                _openAnimation.fullPosition = 1f;
                _fadeAnimation.fullPosition = 1f;
            }
        }

        public void ToggleWindow()
        {
            IsOpen = !IsOpen;
        }
    }
}
