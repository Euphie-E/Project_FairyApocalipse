using UnityEngine;
namespace GCamarada
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] private float cooldown;
        private bool audioPlay = true;
        private AudioManager audioManager;

        private void Start()
        {
            audioManager = AudioManager.I;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (audioPlay)
            {
                audioManager.OnShotClip("WalkConcret");
                audioPlay = false;
                Invoke(nameof(ResetAudioPlay), cooldown);
            }
        }

        private void ResetAudioPlay()
        {
            audioPlay = true;
        }

    }
}