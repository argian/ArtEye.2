using UnityEngine;
using TMPro;

namespace ArtEye
{
    // This class kind of breaks SRP but well VRC isn't really SOLID friendly.
    [RequireComponent(typeof(Collider))]
    public class NarrativePlayer : MonoBehaviour
    {
        // These shouldn't be public but it is what it is...
        public GameObject container;

        public AudioSource audioSource;

        public GameObject textCanvas;

        public TMP_Text textComponent;

        public GameObject playButton;

        public GameObject pauseButton;

        [Space] public bool hideText = true;

        public bool autoResume = false;

        bool _isPlaying = false;

        float _playbackTime = 0;


#if UNITY_EDITOR
        [Space, TextArea(10, 25)] public string text;

        [Space] public AudioClip clip;
#endif

        void Start()
        {
            StopAndHidePlayer();

            if (hideText)
                HideText();
        }

        // FIXME old udon code
        /*
        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            base.OnPlayerTriggerEnter(player);

            if (!player.isLocal)
                return;

            ShowPlayerAndResume();
        }
        */

        // FIXME old udon code
        /*
        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            base.OnPlayerTriggerExit(player);

            if (!player.isLocal)
                return;

            StopAndHidePlayer();
        }
        */

        void ShowPlayerAndResume()
        {
            container.SetActive(true);

            if (autoResume && _isPlaying)
                Play();
            else
            {
                if (playButton)
                    playButton.SetActive(true);
                if (pauseButton)
                    pauseButton.SetActive(false);
            }
        }

        void StopAndHidePlayer()
        {
            if (_isPlaying)
            {
                Stop();

                if (_playbackTime < Mathf.Epsilon) // Finished playing
                    _isPlaying = false;
            }

            container.SetActive(false);
        }

        public void Play()
        {
            audioSource.time = _playbackTime;
            audioSource.Play();

            _isPlaying = true;
        }

        public void Replay()
        {
            _playbackTime = 0;
            Play();
        }

        public void Pause()
        {
            Stop();
            _isPlaying = false;
        }

        void Stop()
        {
            _playbackTime = audioSource.time;
            audioSource.Stop();
        }

        public void ShowText()
        {
            textCanvas.SetActive(true);
        }

        public void HideText()
        {
            textCanvas.SetActive(false);
        }
    }
}