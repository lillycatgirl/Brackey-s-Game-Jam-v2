using UnityEngine;

namespace Gameplay.Monster
{
    public class MonsterLeg : MonoBehaviour
    {
        public AudioSource audioSource;
        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void PlayAudio()
        {
            audioSource.pitch = Random.Range(0.8f, 1.05f);
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
