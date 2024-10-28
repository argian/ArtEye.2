using UnityEngine;

namespace ArtEye.TheGreatWaveOfCanagawa
{
    public class AnimationSwitch : MonoBehaviour
    {
        [SerializeField] private Animator targetAnimator;
        [SerializeField] private string triggerName;

        public void Interact()
        {
            targetAnimator.SetTrigger(triggerName);
        }
    }
}