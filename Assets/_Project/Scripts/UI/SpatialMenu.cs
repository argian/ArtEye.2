using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtEye
{
    public class SpatialMenu : MonoBehaviour
    {
        [SerializeField] private InputActionReference menuActionRef;
        [SerializeField] private ActionTriggerSO menuActionTrigger;

        private void Awake()
        {
            menuActionRef.action.performed += TriggerMenu;
            menuActionTrigger.Performed += TriggerMenu;
            
            gameObject.SetActive(false);
        }

        private void TriggerMenu(InputAction.CallbackContext ctx) => TriggerMenu();

        private void TriggerMenu() => gameObject.SetActive(!gameObject.activeSelf);
    }
}
