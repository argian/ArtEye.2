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

        private bool _fadeInProgress;

        private Material _material;

        private void Awake()
        {
            _material = cover.GetComponent<Renderer>().material;
        }

        private void OnEnable()
        {
            TeleporterBase.OnTeleportationStart += FadeIn;
            TeleporterBase.OnTeleportationEnd += FadeOut;
            SceneLoader.OnLoadEnd += FadeOut;
        }

        private void OnDisable()
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

            _material.SetFloat(_alpha, 0);
            cover.SetActive(true);

            _material.DOFloat(1, _alpha, _speed)
                     .SetEase(_ease)
                     .OnComplete(() => callback?.Invoke());
        }

        private void FadeOut()
        {
            if (!_fadeInProgress)
                return;

            _material.SetFloat(_alpha, 1);
            cover.SetActive(true);

            _material.DOFloat(0, _alpha, _speed)
                     .SetDelay(_delay)
                     .SetEase(_ease)
                     .OnComplete(() => {
                         cover.SetActive(false);
                         _fadeInProgress = false;
                     });
        }
    }
}
