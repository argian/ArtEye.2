using UnityEngine;

namespace ArtEye.TheLastSupper
{
	public class DimensionsRenderManager : MonoBehaviour
	{
		[SerializeField] private GameObject dummy;
		[SerializeField] private MeshRenderer wipeoutMesh;
		[SerializeField] private Shader wipeoutShader;
		[SerializeField] private Shader bakeShader;
		[SerializeField] private Shader comparatorShader;
		[SerializeField] private Shader bakeFinal;

		void Start()
		{
			InitializeDimensionQueue();
		}

		void InitializeDimensionQueue()
		{
			DimensionSorter[] sorters = GetComponentsInChildren<DimensionSorter>();

			//calculate total render space needed for the queue
			int currentQueue = 2000 - (sorters.Length + 1) * 5;

			for (int i = 0; i < sorters.Length; i++)
			{
				sorters[i].SetDiscardQueue(currentQueue++);
				AddMaterialWithShader(wipeoutMesh, wipeoutShader, currentQueue++);

				// bake all previous dimensions
				for (int j = 0; j < i; j++)
				{
					for (int k = 0; k < sorters[j].portals.Length; k++)
					{
						// TODO comparatorShader needs tweaks
						// if (j == 1)
						// AddMaterialWithShader(sorters[j].portals[k].GetRenderer(), comparatorShader, ++currentQueue);
						// else
						AddMaterialWithShader(sorters[j].portals[k].GetRenderer(), bakeShader, currentQueue);
					}
				}

				//first iteration requires 1 step less
				if (i > 0)
					currentQueue++;

				sorters[i].SetAcceptQueue(currentQueue++);
				for (int j = 0; j < sorters[i].portals.Length; j++)
					AddMaterialWithShader(sorters[i].portals[j].GetRenderer(), bakeShader, currentQueue);

				currentQueue++;
			}

			AddMaterialWithShader(wipeoutMesh, wipeoutShader, currentQueue++);

			//last iteration bakes portals diffrently
			for (int i = 0; i < sorters.Length; i++)
			for (int j = 0; j < sorters[i].portals.Length; j++)
				AddMaterialWithShader(sorters[i].portals[j].GetRenderer(), bakeFinal, currentQueue);
		}

		private void AddMaterialWithShader(MeshRenderer r, Shader shader, int renderQueue)
		{
			Material[] materials = r.materials;
			int materialsCount = materials.Length;

			// can't create new Material instance or Instantiate(Material)
			// FIXME old udon code
			GameObject tmp = Instantiate(dummy);
			Material newMaterial = tmp.GetComponent<Renderer>().material;
			newMaterial.shader = shader;
			newMaterial.renderQueue = renderQueue;
			Destroy(tmp);

			Material[] newMaterials = new Material[materialsCount + 1];
			materials.CopyTo(newMaterials, 0);

			newMaterials[materialsCount] = newMaterial;
			r.materials = newMaterials;
		}
	}
}