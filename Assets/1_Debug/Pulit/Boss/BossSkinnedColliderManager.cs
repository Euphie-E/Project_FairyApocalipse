
using System.Collections.Generic;
using UnityEngine;

public class BossSkinnedColliderManager : MonoBehaviour
{
    [Header("Atualizacao dos Colliders")]
    [Min(1)]
    public int updateEveryNFrames = 1;

    private class ColliderData
    {
        public SkinnedMeshRenderer skinnedMesh;
        public MeshCollider meshCollider;
        public Mesh bakedMesh;
    }

    private readonly List<ColliderData> colliders = new();

    private void Awake()
    {
        RegisterColliders();
    }

    private void RegisterColliders()
    {
        SkinnedMeshRenderer[] meshes =
            GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer skinned in meshes)
        {
            MeshCollider meshCollider =
                skinned.GetComponent<MeshCollider>();

            if (meshCollider == null)
                continue;

            Mesh bakedMesh = new Mesh();
            bakedMesh.MarkDynamic();

            ColliderData data = new ColliderData
            {
                skinnedMesh = skinned,
                meshCollider = meshCollider,
                bakedMesh = bakedMesh
            };

            colliders.Add(data);
        }

        Debug.Log(
            "Boss: " + colliders.Count +
            " MeshColliders registrados.",
            this
        );
    }

    private void LateUpdate()
    {
        if (Time.frameCount % Mathf.Max(1, updateEveryNFrames) != 0)
            return;

        foreach (ColliderData data in colliders)
        {
            if (data.skinnedMesh == null ||
                data.meshCollider == null)
                continue;

            if (!data.skinnedMesh.gameObject.activeInHierarchy)
                continue;

            if (!data.meshCollider.enabled)
                continue;

            data.skinnedMesh.BakeMesh(
                data.bakedMesh, false
            );

            data.meshCollider.sharedMesh = null;
            data.meshCollider.sharedMesh = data.bakedMesh;
        }
    }

    private void OnDestroy()
    {
        foreach (ColliderData data in colliders)
        {
            if (data.meshCollider != null &&
                data.meshCollider.sharedMesh == data.bakedMesh)
            {
                data.meshCollider.sharedMesh = null;
            }

            if (data.bakedMesh != null)
                Destroy(data.bakedMesh);
        }

        colliders.Clear();
    }
}
