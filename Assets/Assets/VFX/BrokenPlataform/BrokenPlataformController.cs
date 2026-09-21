using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

public class BrokenPlataformController : MonoBehaviour
{
    public float timerDestroy = 0.5f;
    public VisualEffect visualEffect;

    private int quantVolume = 0;
    private bool played = false;

    void Awake()
    {
        quantVolume = transform.childCount;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(DestroyPlataform());
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && played == false)
        {
            played = true;
            StartCoroutine(DestroyPlataform());
        }
    }*/

    public IEnumerator DestroyPlataform()
    {
        yield return new WaitForSeconds(timerDestroy);

        for (int i = 1; i < quantVolume; i++)
        {
            transform.GetChild(i).GetComponent<MeshCollider>().convex = true;
            transform.GetChild(i).GetComponent<Rigidbody>().useGravity = true;
            transform.GetChild(i).GetComponent<Rigidbody>().isKinematic = false;

            Destroy(transform.GetChild(i).gameObject, Random.Range(3, 12));
        }

        if (visualEffect != null && played == false)
        {
            played = true;
            visualEffect.Play();
        }

        Destroy(gameObject, 20);
    }
}
