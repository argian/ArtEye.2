using UnityEngine;

namespace ArtEye.PersistanceOfMemory
{
	public class ClockCentralizer : ShaderPasser
	{
		public float threshold;

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
			MainMaterial.SetVector(PropertyIDs[0],
				new Vector4(transform.position.x,
					transform.position.y + (threshold - 0.5f) * transform.localScale.y * 2, transform.position.z, 0));
		}
	}
}