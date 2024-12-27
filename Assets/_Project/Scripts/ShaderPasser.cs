using UnityEngine;

namespace ArtEye
{
    //this is the base class
    //for EVERY script that passes data to shaders
    public abstract class ShaderPasser : MonoBehaviour
    {
        public Material MainMaterial;

        protected string[] PropertyNames;
        protected int[] PropertyIDs;
        void Start()
        {
            BakePropertyNames();
            BakePropertyIDs();
            FakeStart();
        }

        //use this as a lazy method of expanding upon Start()
        protected abstract void FakeStart();

        //trust me it is that 1% scenario when hardcoding values is better then setting in inspector, as they are shader based, not material based
        protected abstract void BakePropertyNames();

        //we need to do that, it's just how shaders works
        void BakePropertyIDs()
        {
            //Shader shader = MainMaterial.shader;
            for (int i = 0; i < PropertyNames.Length; ++i)
            {
                PropertyIDs[i] = Shader.PropertyToID(PropertyNames[i]);
            }
        }

        void Update()
        {
            if (!MainMaterial)
                return;

            PassToRender();
        }

        //this thing has to be custom-made every time because of different data formats
        protected abstract void PassToRender();
    }
}
