using UnityEngine;

namespace ArtEye
{
    public class ClockCentralizer : ShaderPasser
    {
		public float meltingThreshold;

		protected override void BakePropertyNames()
		{
			PropertyNames = new string[1];
			PropertyIDs = new int[1];

			PropertyNames[0] = "_CenterPos";
		}

		protected override void FakeStart()
		{

		}

		protected override void PassToRender()
		{
			MainMaterial.SetVector(PropertyIDs[0], new Vector4(transform.position.x, transform.position.y + (meltingThreshold - 0.5f) * transform.lossyScale.y * 2, transform.position.z, 0));
		}
	}
}
