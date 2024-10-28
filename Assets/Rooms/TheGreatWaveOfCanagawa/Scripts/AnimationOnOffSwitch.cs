using UnityEngine;

namespace ArtEye.TheGreatWaveOfCanagawa
{
    public class AnimationOnOffSwitch : MonoBehaviour
    {
        [SerializeField] private Animator targetAnimator;
        [SerializeField] private string boolName;
        public bool isOn;

        private void Start()
        {
            targetAnimator.SetBool(boolName, isOn);
        }

        public void Interact()
        {
            isOn = !isOn;
            targetAnimator.SetBool(boolName, isOn);
        }
    }
}