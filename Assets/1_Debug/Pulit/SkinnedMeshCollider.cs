
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class SkinnedMeshCollider : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;

    private MeshCollider meshCollider;
    private Mesh bakedMesh;

    void Start()
    {
        if (skinnedMesh == null)
            skinnedMesh = GetComponent<SkinnedMeshRenderer>();

        meshCollider = GetComponent<MeshCollider>();

        bakedMesh = new Mesh();
        bakedMesh.MarkDynamic();
    }

    void LateUpdate()
    {
        if (skinnedMesh == null) return;

        // Evita incluir a escala do Transform no BakeMesh
        skinnedMesh.BakeMesh(bakedMesh, false);

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = bakedMesh;
    }

    void OnDestroy()
    {
        if (bakedMesh != null)
            Destroy(bakedMesh);
    }
}
