
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class SkinnedMeshCollider : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;

    private MeshCollider meshCollider;
    private Mesh bakedMesh;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();

        bakedMesh = new Mesh();
        bakedMesh.MarkDynamic();
    }

    void LateUpdate()
    {
        if (skinnedMesh == null) return;

        skinnedMesh.BakeMesh(bakedMesh);

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = bakedMesh;
    }

    void OnDestroy()
    {
        if (bakedMesh != null)
            Destroy(bakedMesh);
    }
}
