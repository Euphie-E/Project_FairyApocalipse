using UnityEngine;

public class ChamaoBrilho : MonoBehaviour
{
    public ParticleSystem brilho;
    void OnTriggerEnter(Collider other)
    {
        brilho.Play();
    }
}
