using Unity.VRTemplate;
using UnityEngine;

namespace ArtEye.Warhol
{
    public class Cooker : MonoBehaviour
    {
        [field: SerializeField] private bool IsOn { get; set; }

        [SerializeField] private Transform cookerOutput;
        [SerializeField] private GameObject[] canPrefabs;
        [SerializeField] private XRKnob[] knobs;
        [SerializeField] private ConveyorBelt[] conveyorBelts;
	
        [SerializeField] private float createCanCooldown;
        
        private bool CanInside {
            get => _canInside;
            set {
                _canInside = value;
                foreach (ConveyorBelt conveyorBelt in conveyorBelts)
                    conveyorBelt.IsOn = !value;
            }
        }
        
        private float _creationCooldown;
        private bool _canInside;

        private void Update()
        {
            if (!IsOn)
                return;

            if (_creationCooldown > 0)
            {
                _creationCooldown -= Time.deltaTime;
                return;
            }

            if (!CanInside)
                return;

            CreateCan();
            _creationCooldown = createCanCooldown;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (CanInside || !other.attachedRigidbody.GetComponent<CampbellCan>())
                return;
            
            Destroy(other.gameObject);
            CanInside = true;
        }

        private void CreateCan()
        {
            int canNumber = 0;
            for (int i = 0; i < knobs.Length; i++)
            {
                int knobValue = knobs[i].value > .5f ? 1 : 0;
                canNumber += knobValue * (1 << i);
            }

            Instantiate(canPrefabs[canNumber], cookerOutput.position, cookerOutput.rotation, transform.root);
            CanInside = false;
        }

        public void CookerToggle()
        {
            IsOn = !IsOn;
        }
    }
}
