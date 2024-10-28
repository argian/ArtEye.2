using UnityEngine;

namespace ArtEye.PersistanceOfMemory
{
	public class ClockAnimator : MonoBehaviour
	{
		[SerializeField] private Vector3[] startingRot;
		[SerializeField] private Transform[] animObjects;
		[SerializeField] private Vector3[] speed;

		private void Start()
		{
			startingRot = new Vector3[animObjects.Length];
			for (int i = 0; i < animObjects.Length; i++)
			{
				startingRot[i] = animObjects[i].localRotation.eulerAngles;
			}
		}

		public void reanimate(float progress)
		{
			for (int i = 0; i < animObjects.Length; i++)
			{
				animObjects[i].localRotation = Quaternion.Euler(startingRot[i].x + progress * speed[i].x * 360,
					startingRot[i].y + progress * speed[i].y * 360, startingRot[i].z + progress * speed[i].z * 360);
			}
		}
	}
}