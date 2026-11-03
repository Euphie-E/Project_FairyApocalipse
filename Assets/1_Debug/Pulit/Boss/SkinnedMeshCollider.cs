
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class SkinnedMeshCollider : MonoBehaviour
{
    private SkinnedMeshRenderer skinnedMesh;
    private MeshCollider meshCollider;
    private Mesh bakedMesh;

    void Start()
    {
       
         skinnedMesh = GetComponent<SkinnedMeshRenderer>();

        meshCollider = GetComponent<MeshCollider>();

        bakedMesh = new Mesh();
        bakedMesh.MarkDynamic();
    }

    void LateUpdate()
    {
               
        skinnedMesh.BakeMesh(bakedMesh, false);

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = bakedMesh;
    }

    void OnDestroy()
    {
    
            Destroy(bakedMesh);
    }
}
