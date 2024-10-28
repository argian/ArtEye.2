using UnityEngine;

namespace ArtEye.CampbellsSoupCans
{
	public class Cooker : MonoBehaviour
	{
		[SerializeField] private bool isOn;

		[SerializeField] private Transform cookerOutput;
		[SerializeField] private GameObject[] canPrefabs;
		[SerializeField] private Knob[] knobs;
		[SerializeField] private ConveyorBelt[] conveyorBelts;

		[SerializeField] private float createCanCooldown;
		private float currentTime;

		[SerializeField] private bool canInside;

		private bool CanInside
		{
			get { return canInside; }
			set
			{
				canInside = value;
				foreach (var t in conveyorBelts)
					t.Activated(!value);
			}
		}

		private void Update()
		{
			if (!isOn)
				return;

			currentTime -= Time.deltaTime;

			if (currentTime > 0 || !CanInside)
				return;

			CreateCan();
			currentTime = createCanCooldown;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (CanInside)
				return;

			if (!other.GetComponent<CampbellCan>())
				return;

			Debug.Log($"{other} can loaded to cooker");
			Destroy(other.gameObject);
			CanInside = true;
		}

		void CreateCan()
		{
			int canNumber = 0;
			for (int i = 0; i < knobs.Length; i++)
				canNumber += int.Parse(knobs[i].GetValue()) * (1 << i);

			Instantiate(canPrefabs[canNumber], cookerOutput.position, cookerOutput.rotation);
			CanInside = false;
		}

		public void CookerToggle()
		{
			isOn = !isOn;
		}
	}
}