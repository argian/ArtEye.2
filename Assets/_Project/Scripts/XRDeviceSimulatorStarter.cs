using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

namespace ArtEye
{
    public class XRDeviceSimulatorStarter : MonoBehaviour
    {
        [SerializeField] private GameObject XRDeviceSimulatorPrefab;

        private GameObject _activeSimulator;

        private async void Start()
        {
            await UniTask.NextFrame();

            if (!XRSettings.isDeviceActive)
                SetupSimulator();
        }

        private void SetupSimulator()
        {
            if (!XRDeviceSimulatorPrefab)
                return;

            _activeSimulator = Instantiate(XRDeviceSimulatorPrefab, transform);
            _activeSimulator.name = XRDeviceSimulatorPrefab.name;
        }

        [ContextMenu("Reset Simulator")]
        public async void ResetSimulator()
        {
            if (!_activeSimulator)
                return;

            Destroy(_activeSimulator);

            await UniTask.NextFrame();

            SetupSimulator();
        }
    }
}