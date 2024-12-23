using TMPro;
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

        [Space]
        [SerializeField] private TMP_Text header;
        [SerializeField] private GameObject backButton;

        [SerializeField] private MenuView currentView;
        private MenuView _previousView;

        Transform _hmdTransform;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            menuActionRef.action.performed += TriggerMenu;
            menuActionTrigger.Performed += TriggerMenu;

            _hmdTransform = Camera.main.transform;

            backButton.SetActive(false);
            gameObject.SetActive(false);
        }

        private void OnEnable() => Reposition();

        private void Reposition()
        {
            Vector3 newPosition = _hmdTransform.position + (_hmdTransform.forward * offset.z);
            newPosition.x += offset.x;
            newPosition.y *= .5f;
            newPosition.y += offset.y;

            transform.position = newPosition;
        }

        private void TriggerMenu(InputAction.CallbackContext ctx) => TriggerMenu();

        private void TriggerMenu() => gameObject.SetActive(!gameObject.activeSelf);

        public void ChangeView(MenuView view)
        {
            _previousView = currentView;
            
            currentView.gameObject.SetActive(false);
            currentView = view;
            currentView.gameObject.SetActive(true);

            header.SetText(currentView.Header);

            backButton.SetActive(true);
        }

        public void GoBack()
        {
            if (!_previousView)
                return;

            currentView.gameObject.SetActive(false);
            currentView = _previousView;
            currentView.gameObject.SetActive(true);

            header.SetText(currentView.Header);

            backButton.SetActive(false);
            _previousView = null;
        }
    }
}