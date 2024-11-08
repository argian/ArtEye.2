using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;

namespace ArtEye
{
    public class XRDeviceSimulatorStarter : MonoBehaviour
    {
        private GameObject _simulatorPrefab;
        private GameObject _activeSimulator;

        public void SetPrefab(GameObject simulatorPrefab)
        {
            _simulatorPrefab = simulatorPrefab;
        }

        private async void Start()
        {
            await UniTask.NextFrame();

            if (!XRSettings.isDeviceActive)
                SetupSimulator();
        }

        private void SetupSimulator()
        {
            if (!_simulatorPrefab)
                return;

            _activeSimulator = Instantiate(_simulatorPrefab, transform);
            _activeSimulator.name = _simulatorPrefab.name;
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