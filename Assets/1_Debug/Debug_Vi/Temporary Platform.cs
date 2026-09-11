using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class TemporaryPlatform : MonoBehaviour
{
    private bool isBreaking = false;

    [Tooltip("Quanto tempo a plataforma vai quebrar após pisar nela.")]
    [SerializeField] private float breakTime = 3;
    private float breakTimer = 3;

    [Tooltip("Mínimo de tremor quando quebrar de 0 a 45 (em graus).")]
    [SerializeField] private float tremorMin = 3;

    [Tooltip("Máximo de tremor quando quebrar de 0 a 45 (em graus).")]
    [SerializeField] private float tremorMax = 3;

    [Tooltip("Tempo em que o tremor atinge o máximo antes de quebrar.")]
    [SerializeField] private float tremorMaxTime = 3;

    [Tooltip("O quanto vai frequentemente oscilar do tremor atual.")]
    [SerializeField] private float tremorOscillation = 3;

    [Tooltip("Marque caso esteja tremendo pro lado errado.")]
    [SerializeField] private float flipAxis = 3;

    [SerializeField] private bool autoRestore = false;
    private bool isRestoring = false;
    [SerializeField] private float restoreTime = 3;
    private float restoreTimer = 3;

    private MeshRenderer platformMesh;
    private Collider platformCollider;
    
    void Start()
    {
        platformMesh = gameObject.GetComponent<MeshRenderer>();
        platformCollider = gameObject.GetComponent<Collider>();
        breakTimer = breakTime;
        restoreTimer = restoreTime;
    }

    void Update()
    {
        Breaking();
        Restoring();
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("colidi com o " + hit.gameObject.name);
        /* if (hit.normal.y > 0.5f) // Confere se o player ta em cima
        {
            
        } */
        BreakAndRestorePlatform();
    }

    private void Breaking()
    {
        if (isBreaking)                         // FALTA TREMOR
        {
            breakTimer -= Time.deltaTime;

            if (breakTimer <= 0)
            {
                platformCollider.enabled = false;
                platformMesh.enabled = false;

                breakTimer = breakTime;

                isBreaking = false;
                isRestoring = autoRestore;
            }
        }
    }

    private void Restoring()
    {
        if (isRestoring)
        {
            restoreTimer -= Time.deltaTime;

            if (restoreTimer <= 0)
            {
                platformCollider.enabled = true;
                platformMesh.enabled = true;

                restoreTimer = restoreTime;

                isRestoring = false;
            }
        }
    }

    public void BreakPlatform()
    {
        isBreaking = true;
    }

    public void BreakPlatform(float breakTime)
    {
        isBreaking = true;
        this.breakTime = breakTime;
        breakTimer = breakTime;
    }


    public void BreakAndRestorePlatform()
    {
        isBreaking = true;
        autoRestore = true;
    }

    public void RestorePlatform()
    {
        isRestoring = true;
    }

    public void RestorePlatform(float restoreTime)
    {
        isBreaking = true;
        this.restoreTime = restoreTime;
        restoreTimer = restoreTime;
    }
}
