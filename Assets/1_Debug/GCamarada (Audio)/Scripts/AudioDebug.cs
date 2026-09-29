using UnityEngine;

namespace GCamarada
{
    public class AudioDebug : MonoBehaviour
    {
        public AudioManager audioManager;

        private void Start()
        {
            audioManager = AudioManager.I;
            audioManager.PlayClip("Teste", true);
            audioManager.PlayClip("teste2", true);
        }


    }
}
