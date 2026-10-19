using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[AddComponentMenu("VFX/Utility/VFX Skinned Mesh Binder")]
public class VFXSkinnedMeshBinder : VFXBinderBase
{
    public SkinnedMeshRenderer skinnedMesh;
    public string MeshProperty = "SkinMesh";

    private ExposedProperty _meshExp;
    private Mesh bakedMesh;

    protected override void OnEnable()
    {
        base.OnEnable();
        _meshExp = MeshProperty;
        bakedMesh = new Mesh();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (bakedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(bakedMesh);
            else
                DestroyImmediate(bakedMesh);

            bakedMesh = null;
        }
    }

    public override bool IsValid(VisualEffect component)
    {
        return skinnedMesh != null &&
               component.HasMesh(_meshExp);
    }

    public override void UpdateBinding(VisualEffect component)
    {
        if (skinnedMesh == null || bakedMesh == null)
            return;

        skinnedMesh.BakeMesh(bakedMesh, false);

        bakedMesh.bounds = skinnedMesh.localBounds;

        component.SetMesh(_meshExp, bakedMesh);
    }

    public override string ToString()
    {
        return $"VFX Skinned Mesh Binder : {MeshProperty}";
    }
}
