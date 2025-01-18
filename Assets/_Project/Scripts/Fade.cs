using System;
using ArtEye.Teleportation;
using UnityEngine;
using DG.Tweening;

namespace ArtEye
{
    public class Fade : MonoBehaviour
    {
        private static readonly int _alpha = Shader.PropertyToID("_Alpha");

        [SerializeField] private GameObject cover;
        [SerializeField] private float _speed = .5f;
        [SerializeField] private float _delay = .25f;
        [SerializeField] private Ease _ease = Ease.Linear;
        [Space]
        [SerializeField] private bool animate;

        private bool _fadeInProgress;

        private Material _material;

        private Tween _tween;

        private void Awake()
        {
            TeleporterBase.OnTeleportationStart += FadeIn;
            TeleporterBase.OnTeleportationEnd += FadeOut;
            SceneLoader.OnLoadEnd += FadeOut;

            _material = cover.GetComponent<Renderer>().material;

            SetCover(false, 0);
        }

        private void OnDestroy()
        {
            TeleporterBase.OnTeleportationStart -= FadeIn;
            TeleporterBase.OnTeleportationEnd -= FadeOut;
            SceneLoader.OnLoadEnd -= FadeOut;
        }

        private void FadeIn(Action callback)
        {
            if (_fadeInProgress)
                return;

            _fadeInProgress = true;

            if (!animate)
            {
                callback?.Invoke();
                return;
            }    

            cover.SetActive(true);

            _tween = _material.DOFloat(1, _alpha, _speed)
                              .SetEase(_ease)
                              .OnComplete(() => callback?.Invoke());
        }

        private void FadeOut()
        {
            if (!_fadeInProgress)
                return;

            if (!animate)
            {
                _fadeInProgress = false;
                return;
            }

            cover.SetActive(true);

            _tween = _material.DOFloat(0, _alpha, _speed)
                              .SetDelay(_delay)
                              .SetEase(_ease)
                              .OnComplete(() => {
                                  cover.SetActive(false);
                                  _fadeInProgress = false;
                              });
        }

        public void SetCover(bool enabled, float value)
        {
            _material.SetFloat(_alpha, value);
            cover.SetActive(enabled);
        }

        public void ForceComplete()
        {
            if (_tween == null || !_fadeInProgress)
                return;

            _tween.Complete();
        }
    }
}
