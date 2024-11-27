using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ArtEye
{
    // This class kind of breaks SRP but well VRC isn't really SOLID friendly.
    [RequireComponent(typeof(Collider))]
    public class NarrativePlayer : MonoBehaviour
    {
        // TODO make private
        [SerializeField] private GameObject container;
        public AudioSource audioSource;
        [SerializeField] private GameObject textScrollView;
        public TMP_Text textComponent;
        [SerializeField] private Image playButtonImage;
        [SerializeField] private Slider seekSlider;

        [SerializeField] private Sprite playIcon;
        [SerializeField] private Sprite pauseIcon;
        [SerializeField] private Sprite replayIcon;

        [Space] public bool hideText = true;

        [SerializeField] private bool autoResume;

        private bool _isPlaying;
        private float _playbackTime;
        private bool _showSubtitles;

#if UNITY_EDITOR
        [Space, TextArea(10, 25)] public string text;
        [Space] public AudioClip clip;
#endif

        private void OnDisable()
        {
            audioSource.Stop();
        }

        private void Start()
        {
            StopAndHidePlayer();
        }

        private void Update()
        {
            if (audioSource.isPlaying)
                seekSlider.SetValueWithoutNotify(audioSource.time / audioSource.clip.length);
        }

        public void ShowPlayerAndResume()
        {
            container.SetActive(true);

            if (autoResume && _isPlaying)
                Play();
        }

        private void StopAndHidePlayer()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            container.SetActive(false);
        }

        public void Play()
        {
            playButtonImage.sprite = pauseIcon;
            if (Mathf.Approximately(audioSource.time, audioSource.clip.length))
                _playbackTime = 0;
            audioSource.time = _playbackTime;
            audioSource.Play();
        }

        public void Pause()
        {
            _playbackTime = audioSource.time;
            audioSource.Pause();
        }

        public void ToggleSubtitles()
        {
            _showSubtitles = !_showSubtitles;
            textScrollView.SetActive(_showSubtitles);
        }

        public void Seek(float playbackPercent)
        {
            audioSource.time = audioSource.clip.length / playbackPercent;
        }
    }
}