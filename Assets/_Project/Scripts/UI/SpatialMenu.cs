using UnityEngine;
using UnityEngine.InputSystem;

namespace ArtEye
{
    public class SpatialMenu : MonoBehaviour
    {
        [SerializeField] private InputActionReference menuActionRef;
        [SerializeField] private ActionTriggerSO menuActionTrigger;

        [Space]
        [SerializeField] private Vector3 offset = new(0, .2f, 1.25f);

        Transform hmdTransform;

        private void Awake()
        {
            menuActionRef.action.performed += TriggerMenu;
            menuActionTrigger.Performed += TriggerMenu;

            hmdTransform = Camera.main.transform;

            gameObject.SetActive(false);
        }

        private void OnEnable() => Reposition();

        private void Reposition()
        {
            Vector3 newPosition = hmdTransform.position + (hmdTransform.forward * offset.z);
            newPosition.x += offset.x;
            newPosition.y *= .5f;
            newPosition.y += offset.y;

            transform.position = newPosition;
        }

        private void TriggerMenu(InputAction.CallbackContext ctx) => TriggerMenu();

        private void TriggerMenu() => gameObject.SetActive(!gameObject.activeSelf);
    }
}
