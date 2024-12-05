using DG.Tweening;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ArtEye
{
    // This class kind of breaks SRP but well VRC isn't really SOLID friendly.
    [RequireComponent(typeof(Collider))]
    public class NarrativePlayer : MonoBehaviour
    {
        // container
        [SerializeField] private CanvasGroup containerCanvasGroup;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private TMP_Text textComponent;
        
        [SerializeField] private RectTransform textScrollView;

        [SerializeField] private TMP_Text playbackTimeText;
        [SerializeField] private Slider seekSlider;

        // play button
        [SerializeField] private Image playButtonImage;
        [SerializeField] private Sprite playIcon;
        [SerializeField] private Sprite pauseIcon;
        [SerializeField] private Sprite replayIcon;

        private Tweener _showContainerAnimation;
        private Tweener _showSubtitlesAnimation;

        private bool _isVisible;
        private float _playbackTime;
        private bool _showSubtitles;

#if UNITY_EDITOR
        [Space, TextArea(10, 25)] public string text;
        [Space] public AudioClip clip;
#endif

        private void Awake()
        {
            _showContainerAnimation = containerCanvasGroup.DOFade(1f, .3f)
                .SetEase(Ease.InOutSine)
                .Pause()
                .SetAutoKill(false);
            
            _showSubtitlesAnimation = textScrollView.DOSizeDelta(new Vector3(0f, 300f, 0f), 1f)
                .SetEase(Ease.OutSine)
                .Pause()
                .SetAutoKill(false);
        }

        private void Start()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            if (!_isVisible)
                containerCanvasGroup.alpha = 0;
            textScrollView.sizeDelta = Vector2.zero;
        }

        private void Update()
        {
            if (_isVisible && audioSource.isPlaying)
                UpdatePlaybackSliderAndText();

            if (Mathf.Approximately(audioSource.time, audioSource.clip.length))
                playButtonImage.sprite = replayIcon;
        }
        
        private void OnTriggerExit(Collider other)
        {
            HidePlayer();
        }
        
        public void ToggleVisibility()
        {
            if (_isVisible)
                HidePlayer();
            else
                ShowPlayer();

            _isVisible = !_isVisible;
        }

        public void TogglePlay()
        {
            if (audioSource.isPlaying)
                Pause();
            else
                PlayOrRestart();
        }

        public void ToggleSubtitles()
        {
            _showSubtitles = !_showSubtitles;
            if (_showSubtitles)
                _showSubtitlesAnimation.PlayForward();
            else
                _showSubtitlesAnimation.PlayBackwards();
        }

        private void UpdatePlaybackSliderAndText()
        {
            seekSlider.SetValueWithoutNotify(audioSource.time / audioSource.clip.length);
            
            int minutes = Mathf.FloorToInt(audioSource.time / 60);
            int seconds = Mathf.FloorToInt(audioSource.time % 60);
            playbackTimeText.text = $"{minutes:00}:{seconds:00}";
        }

        private async void ShowPlayer()
        {
            _showContainerAnimation.PlayForward();
            await _showContainerAnimation.AsyncWaitForCompletion();

            PlayOrRestart();
        }

        private void HidePlayer()
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
            
            _showContainerAnimation.PlayBackwards();
        }

        private void PlayOrRestart()
        {
            playButtonImage.sprite = pauseIcon;
            if (audioSource.time == 0)
                _playbackTime = 0;
            audioSource.time = _playbackTime;
            audioSource.Play();
        }

        private void Pause()
        {
            _playbackTime = audioSource.time;
            playButtonImage.sprite = playIcon;
            audioSource.Pause();
        }

        public void Seek(float playbackPercent)
        {
            audioSource.time = audioSource.clip.length * playbackPercent;
            _playbackTime = audioSource.time;
            
            UpdatePlaybackSliderAndText();
        }
    }
}