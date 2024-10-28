using System;
using UnityEngine;

namespace ArtEye.CampbellsSoupCans
{
	public class Knob : MonoBehaviour
	{
		[SerializeField] private float[] angles;
		[SerializeField] private string[] values;

		[SerializeField] private float currentAngle;

		public string GetValue()
		{
			return values[FindNearestAngle(currentAngle)];
		}

		private int FindNearestAngle(float target)
		{
			int nearestIndex = 0;
			float minDifference = Math.Abs(target - angles[0]);

			for (int i = 1; i < angles.Length; i++)
			{
				float difference = Math.Abs(target - angles[i]);

				if (difference < minDifference)
				{
					minDifference = difference;
					nearestIndex = i;
				}
			}

			return nearestIndex;
		}

		public void SetAngle(float angle)
		{
			currentAngle = angle;
		}
	}
}