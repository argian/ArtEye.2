using System;
using ArtEye.Teleportation;
using UnityEngine;

namespace ArtEye
{
    public class Fade : MonoBehaviour
    {
        private bool _fadeInProgress;
        
        private void OnEnable()
        {
            SceneLoader.OnLoadEnd += FadeIn;
            TeleporterBase.OnTeleportationStart += FadeOut;
            TeleporterBase.OnTeleportationEnd += FadeIn;
        }

        private void OnDisable()
        {
            SceneLoader.OnLoadEnd -= FadeIn;
            TeleporterBase.OnTeleportationStart -= FadeOut;
            TeleporterBase.OnTeleportationEnd -= FadeIn;
        }

        private void FadeIn()
        {
            // TODO fade in logic
            _fadeInProgress = false;
        }

        private void FadeOut(Action callback)
        {
            if (_fadeInProgress)
                return;
            // TODO fade out logic
            _fadeInProgress = true;
            callback?.Invoke();
        }
    }
}
