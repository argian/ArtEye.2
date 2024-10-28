using UnityEngine;

namespace ArtEye.CampbellsSoupCans
{
    public class CookerOnOffButton : MonoBehaviour
    {
        [SerializeField] private Cooker cooker;

        public void Interact()
        {
            cooker.CookerToggle();
        }
    }
}