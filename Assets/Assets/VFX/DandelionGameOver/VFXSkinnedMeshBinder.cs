using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[AddComponentMenu("VFX/Utility/VFX Skinned Mesh Binder")]
public class VFXSkinnedMeshBinder : VFXBinderBase
{
    public SkinnedMeshRenderer skinnedMesh;
    public string MeshProperty = "SkinMesh";

    private ExposedProperty _meshExp;

    protected override void OnEnable()
    {
        base.OnEnable();
        _meshExp = MeshProperty;
    }

    public override bool IsValid(VisualEffect component)
        => skinnedMesh != null && component.HasMesh(_meshExp);

    public override void UpdateBinding(VisualEffect component)
    {
        if (skinnedMesh == null) return;
        var baked = new Mesh();
        skinnedMesh.BakeMesh(baked, true);
        component.SetMesh(_meshExp, baked);
    }

    public override string ToString() => $"VFX Skinned Mesh Binder : {MeshProperty}";
}