using UnityEngine;

namespace ArtEye.PersistanceOfMemory
{
	public class RaySkyboxDatabase : ShaderPasser
	{
		[SerializeField] private SkinnedMeshRenderer treeMesh;
		[SerializeField] private ClockAnimator[] clockAnimators;
		[SerializeField] private ClockCentralizer[] clockCentralizers;
		[SerializeField] private Transform[] markers;
		[SerializeField] private int currentMarker;
		[SerializeField] private float activationDistance = 0.05f;
		[SerializeField] private AnimationCurve lerpCurve;

		// FIXME old udon code
		// private VRCPlayerApi localPlayer;

		[SerializeField] private float YScale;

		[SerializeField] private float lerp;

		public void Reset()
		{
			currentMarker = 0;
			lerp = 0;
		}

		protected override void BakePropertyNames()
		{
			PropertyNames = new string[8];
			PropertyIDs = new int[8];

			PropertyNames[0] = "Spacing1";
			PropertyNames[1] = "Spacing2";
			PropertyNames[2] = "Spacing3";
			PropertyNames[3] = "4";
			PropertyNames[4] = "5";
			PropertyNames[5] = "6";
			PropertyNames[6] = "7";
			PropertyNames[7] = "8";
		}

		protected override void FakeStart()
		{
			// FIXME old udon code
			// localPlayer = Networking.LocalPlayer;

			MainMaterial.SetVector(PropertyIDs[0], new Vector4(0.1f, YScale, 0.1f, lerp));
			MainMaterial.SetVector(PropertyIDs[1], new Vector4(0.1f, YScale, 0.1f, lerp));
			MainMaterial.SetVector(PropertyIDs[2], new Vector4(0.1f, YScale, 0.1f, lerp));
		}

		protected override void PassToRender()
		{
			if (currentMarker == markers.Length - 1)
				return;

			//camera pos and rot
			// FIXME old udon code
			// Vector3 playerHeadPosition = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Origin).position;

			// FIXME old udon code
			// float distanceToNextMarker = Vector3.Distance(markers[currentMarker + 1].position, playerHeadPosition);

			// FIXME old udon code
			// lerp = 1f - distanceToNextMarker / Vector3.Distance(markers[currentMarker].position, markers[currentMarker + 1].position);

			// FIXME old udon code
			/*
			if (distanceToNextMarker <= activationDistance)
			{
				lerp = 1f;
				currentMarker++;
			}
			*/

			lerp = lerp < 0 ? 0 : lerp;
			lerp = lerpCurve.Evaluate(lerp);
			lerp = Mathf.Clamp(lerp, 0.1f, 1f);
			MainMaterial.SetVector(PropertyIDs[currentMarker], new Vector4(lerp, YScale, lerp, lerp));

			for (int i = 0; i < clockAnimators.Length; i++)
			{
				clockAnimators[i].reanimate(lerp);
			}

			float markerPart = 1f / (float)(markers.Length - 1);
			for (int i = 0; i < clockCentralizers.Length; i++)
			{
				clockCentralizers[i].threshold = markerPart * currentMarker + lerp * markerPart;
			}

			if (currentMarker % 2 > 0)
			{
				treeMesh.SetBlendShapeWeight(0, 100 - (markerPart * currentMarker + lerp * markerPart * 400));
			}
			else
			{
				treeMesh.SetBlendShapeWeight(0, markerPart * currentMarker + lerp * markerPart * 400);
			}

			Debug.Log(markerPart * currentMarker + lerp * markerPart * 100);
		}
	}
}