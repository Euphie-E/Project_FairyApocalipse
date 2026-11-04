using UnityEngine;

[ExecuteAlways]
public class TrailUV : MonoBehaviour
{
    [System.Serializable]
    public struct MaterialData
    {
        public MaterialData(TrailRenderer trailRenderer, Material material, Vector2 uvScale, float move)
        {
            mat_trailRenderer = trailRenderer;
            mat_uvTiling = uvScale;
            mat_move = move;
        }
        
        public TrailRenderer mat_trailRenderer;
        [HideInInspector] public Vector2 mat_uvTiling;
        [HideInInspector] public float mat_move;
    }

#if UNITY_EDITOR
#endif
    public Transform mat_moveObject;
    public string mat_shaderPropertyName = "_MoveToMaterialUV";
    public int mat_shaderPropertyID;
    public MaterialData[] mat_materialData = new MaterialData[1] { new MaterialData ( null, null, new Vector2(1, 1), 0f ) };

    private Vector3 mat_beforePosW = Vector3.zero;
    void Start()
    {
        Initialize();
    }

    void LateUpdate()
    {
        if (mat_moveObject == null)
            return;
        if (mat_materialData == null || mat_materialData.Length == 0)
            return;

        Vector3 nowPosW = mat_moveObject.transform.position;
        if (nowPosW == mat_beforePosW)
            return;
        
        float distance = Vector3.Distance(nowPosW, mat_beforePosW);
        mat_beforePosW = nowPosW;

        for (int i = 0; i < mat_materialData.Length; i++)
        {
            if (mat_materialData[i].mat_trailRenderer == null)
                continue;

            mat_materialData[i].mat_move += distance *mat_materialData[i].mat_uvTiling.x;

            if (mat_materialData[i].mat_move > 1f)
            {
                mat_materialData[i].mat_move = mat_materialData[i].mat_move % 1f;
            }

            TrailRenderer trailRenderer = mat_materialData[i].mat_trailRenderer;
            if (trailRenderer != null)
            {
                Material mat = trailRenderer.sharedMaterial;
                if (mat != null)
                {
                    mat.SetFloat(mat_shaderPropertyID, mat_materialData[i].mat_move);
                }
            }
        }
    }

    public void Initialize()
    {
        if (mat_materialData == null || mat_materialData.Length == 0)
            return;
        
        mat_shaderPropertyID = Shader.PropertyToID(mat_shaderPropertyName);

        for (int i = 0; i < mat_materialData.Length; i++)
        {
            mat_materialData[i].mat_move = 0f;
            TrailRenderer trailRenderer = mat_materialData[i].mat_trailRenderer;
            if (trailRenderer != null)
            {
                Material mat = trailRenderer.sharedMaterial;
                if (mat != null)
                {
                    mat_materialData[i].mat_uvTiling = mat.mainTextureScale;
                }
            }
        }
    }
}
