using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public float dissolveRate = 0.1f;
    public float refreshRate = 0.05f;
    public VisualEffect vfxDandelion;

    private Material[] skinnedMaterials;

    void Start()
    {
        if (skinnedMeshRenderer != null)
        {
            skinnedMaterials = skinnedMeshRenderer.materials;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(DissolveCo());
        }
    }

    IEnumerator DissolveCo()
    {
        if(vfxDandelion != null)
        {
            vfxDandelion.Play();
        }

        float counter = 0;
        if (skinnedMaterials.Length > 0)
        {
            while(skinnedMaterials[0].GetFloat("_DissolveAmount") < 1)
            {
                counter += dissolveRate;

                for(int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}
