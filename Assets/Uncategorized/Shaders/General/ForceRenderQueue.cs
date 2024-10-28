using UnityEngine;

public class ForceRenderQueue : MonoBehaviour
{
    public int RenderQueue;
    void Start()
    {
        gameObject.GetComponent<MeshRenderer>().material.renderQueue = RenderQueue;
    }
}
